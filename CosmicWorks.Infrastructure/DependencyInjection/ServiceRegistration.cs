using CosmicWorks.Application.Abstractions;
using CosmicWorks.Application.Events;
using CosmicWorks.Application.Prompting;
using CosmicWorks.Application.UseCases;
using CosmicWorks.Configuration;
using CosmicWorks.Domain.Policies;
using CosmicWorks.Domain.Repositories;
using CosmicWorks.Infrastructure.Integrations.OpenAI;
using CosmicWorks.Infrastructure.Integrations.Persistance.Cosmos;
using CosmicWorks.Infrastructure.Policies;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CosmicWorks.Infrastructure.DependencyInjection;

/// <summary>
/// DI bootstrap for the Infrastructure layer. Registers adapters for:
/// Cosmos DB (repository, similarity search), Azure OpenAI (chat/embeddings),
/// discount policy, and the application use cases/facade.
/// </summary>
public static class ServiceRegistration
{
    public static IServiceCollection AddCosmicWorksInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Options with validation (fail fast on startup)
        services.AddOptions<OpenAISettings>()
                .Bind(configuration.GetSection("OpenAI"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddOptions<CosmosSettings>()
                .Bind(configuration.GetSection("Cosmos"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddOptions<RagSettings>()
                .Bind(configuration.GetSection("Rag"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.Configure<DiscountPolicySettings>(configuration.GetSection("DiscountPolicy"));

        // Discount Policies
        services.AddSingleton<IDiscountPolicy>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<DiscountPolicySettings>>().Value;
            return new CategoryDiscountPolicy(settings);
        });

        // Domain Events
        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();

        // Prompting
        services.AddSingleton<CatalogPromptBuilder>();
        services.AddSingleton<ISystemPromptTemplate>(sp => sp.GetRequiredService<CatalogPromptBuilder>());
        services.AddSingleton<ICatalogContextPrompt>(sp => sp.GetRequiredService<CatalogPromptBuilder>());        
        services.AddSingleton<ICatalogPromptBuilder>(sp => sp.GetRequiredService<CatalogPromptBuilder>());

        // Use-cases
        services.AddSingleton<ApplyProductDiscount>();
        services.AddSingleton<RemoveProductDiscount>();
        services.AddSingleton<ListCategories>();
        services.AddSingleton<ChatWithCatalog>(sp =>
        {
            var rag = sp.GetRequiredService<IOptions<RagSettings>>().Value;
            return new ChatWithCatalog(
                sp.GetRequiredService<IProductSimilaritySearcher>(),
                sp.GetRequiredService<IEmbeddingService>(),
                sp.GetRequiredService<IChatService>(),
                sp.GetRequiredService<ICatalogPromptBuilder>(),
                rag.TopK,
                rag.MinSimilarity);
        });

        // Facade
        services.AddSingleton<CopilotService>(sp =>
            new CopilotService(
                sp.GetRequiredService<ApplyProductDiscount>(),
                sp.GetRequiredService<RemoveProductDiscount>(),
                sp.GetRequiredService<ChatWithCatalog>(),
                sp.GetRequiredService<ListCategories>(),
                sp.GetRequiredService<ICatalogPromptBuilder>()));

        services.AddSingleton<ICopilotService>(sp => sp.GetRequiredService<CopilotService>());        
        services.AddSingleton<ICatalogChat>(sp => sp.GetRequiredService<CopilotService>());
        services.AddSingleton<IDiscountCommands>(sp => sp.GetRequiredService<CopilotService>());
        services.AddSingleton<ICategoryQueries>(sp => sp.GetRequiredService<CopilotService>());

        // HttpClient factory
        services.AddHttpClient();

        // Azure OpenAI adapters
        services.AddSingleton<IEmbeddingService>(sp =>
        {
            var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
            var cfg = sp.GetRequiredService<IOptions<OpenAISettings>>().Value;
            return new AzureOpenAIEmbeddingService(http, cfg);
        });

        services.AddSingleton<IChatService>(sp =>
        {
            var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
            var cfg = sp.GetRequiredService<IOptions<OpenAISettings>>().Value;
            return new AzureOpenAIChatService(http, cfg);
        });

        // Cosmos
        services.AddSingleton(sp =>
        {
            var cfg = sp.GetRequiredService<IOptions<CosmosSettings>>().Value;
            return new CosmosClient(cfg.Endpoint, cfg.Key);
        });

        services.AddSingleton<CosmosProductRepository>(sp =>
        {
            var cfg = sp.GetRequiredService<IOptions<CosmosSettings>>().Value;
            var client = sp.GetRequiredService<CosmosClient>();
            var container = client.GetDatabase(cfg.Database).GetContainer(cfg.Container);
            return new CosmosProductRepository(container, cfg.PartitionKeyPath);
        });

        services.AddSingleton<IProductReader>(sp => sp.GetRequiredService<CosmosProductRepository>());
        services.AddSingleton<IProductWriter>(sp => sp.GetRequiredService<CosmosProductRepository>());        
        services.AddSingleton<IProductRepository>(sp => sp.GetRequiredService<CosmosProductRepository>());

        services.AddSingleton<IProductSimilaritySearcher>(sp =>
        {
            var cfg = sp.GetRequiredService<IOptions<CosmosSettings>>().Value;
            var client = sp.GetRequiredService<CosmosClient>();
            var container = client.GetDatabase(cfg.Database).GetContainer(cfg.Container);
            return new CosmosProductSimilaritySearcher(container);
        });

        return services;
    }
}