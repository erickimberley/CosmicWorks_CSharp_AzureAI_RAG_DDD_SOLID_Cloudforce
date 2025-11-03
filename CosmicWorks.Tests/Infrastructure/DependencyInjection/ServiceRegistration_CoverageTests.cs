using CosmicWorks.Application.Abstractions;
using CosmicWorks.Application.Prompting;
using CosmicWorks.Application.UseCases;
using CosmicWorks.Configuration;
using CosmicWorks.Domain.Policies;
using CosmicWorks.Domain.Repositories;
using CosmicWorks.Infrastructure.DependencyInjection;
using CosmicWorks.Infrastructure.Integrations.Persistance.Cosmos;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace CosmicWorks.Tests.Infrastructure.DependencyInjection;

public class ServiceRegistration_CoverageTests
{
    private static IConfiguration BuildCfg() =>
    new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            // Cosmos
            ["Cosmos:Endpoint"] = "https://example.cosmos.azure.com",
            ["Cosmos:Key"] = "c2VjcmV0LXByaW1hcnkta2V5LXRlc3Q=",
            ["Cosmos:Database"] = "CosmicWorks",
            ["Cosmos:Container"] = "Products",
            ["Cosmos:PartitionKeyPath"] = "/category_id",

            // OpenAI
            ["OpenAI:Endpoint"] = "https://example.openai.azure.com",
            ["OpenAI:ApiVersion"] = "2024-06-01",
            ["OpenAI:EmbeddingDeployment"] = "text-embedding-3-small",
            ["OpenAI:ChatDeployment"] = "gpt-4o-mini",
            ["OpenAI:ApiKey"] = "x",

            // RAG
            ["Rag:TopK"] = "3",
            ["Rag:MinSimilarity"] = "0.25",

            // Discount caps
            ["DiscountPolicy:DefaultCap"] = "0.8",
            ["DiscountPolicy:Caps:Accessories, Helmets"] = "0.30"
        })
        .Build();

    [Fact]
    public void Registers_And_Resolves_All_Core_Services()
    {
        var services = new ServiceCollection();
        services.AddCosmicWorksInfrastructure(BuildCfg());
        var sp = services.BuildServiceProvider();

        // options bound
        sp.GetRequiredService<IOptions<CosmosSettings>>().Value.Database.Should().Be("CosmicWorks");
        sp.GetRequiredService<IOptions<RagSettings>>().Value.TopK.Should().Be(3);

        // discount policy
        sp.GetRequiredService<IDiscountPolicy>().Should().NotBeNull();

        // prompting + use-cases
        sp.GetRequiredService<ICatalogPromptBuilder>().Should().NotBeNull();
        sp.GetRequiredService<CatalogPromptBuilder>().Should().NotBeNull();
        sp.GetRequiredService<ChatWithCatalog>().Should().NotBeNull();
        sp.GetRequiredService<ListCategories>().Should().NotBeNull();
        sp.GetRequiredService<ApplyProductDiscount>().Should().NotBeNull();
        sp.GetRequiredService<RemoveProductDiscount>().Should().NotBeNull();

        // copilot façade implemented by single service
        var copilot = sp.GetRequiredService<ICopilotService>();
        sp.GetRequiredService<ICategoryQueries>().Should().BeSameAs(copilot);
        sp.GetRequiredService<IDiscountCommands>().Should().BeSameAs(copilot);
        sp.GetRequiredService<ICatalogChat>().Should().BeSameAs(copilot);

        // cosmos-backed services are resolvable (no network hit)
        sp.GetRequiredService<CosmosProductRepository>().Should().NotBeNull();
        sp.GetRequiredService<IProductReader>().Should().NotBeNull();
        sp.GetRequiredService<IProductWriter>().Should().NotBeNull();
        sp.GetRequiredService<IProductRepository>().Should().NotBeNull();
        sp.GetRequiredService<IProductSimilaritySearcher>().Should().NotBeNull();
    }
}