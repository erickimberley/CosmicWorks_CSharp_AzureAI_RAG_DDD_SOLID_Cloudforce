using CosmicWorks.Configuration;
using CosmicWorks.Infrastructure.Integrations.OpenAI;
using Xunit;

namespace CosmicWorks.Tests.Infrastructure.Integrations.OpenAI;

public class OpenAISettingsValidationTests
{
    [Fact]
    public void Chat_Throws_When_ApiKey_Missing()
    {
        var http = new HttpClient(new HttpClientHandler());
        var cfg = new OpenAISettings
        {
            Endpoint = "https://example.openai.azure.com",
            ApiVersion = "2024-06-01",
            ChatDeployment = "gpt",
            EmbeddingDeployment = "embed",
            ApiKey = string.Empty // missing
        };

        Assert.Throws<InvalidOperationException>(() => new AzureOpenAIChatService(http, cfg));
    }

    [Fact]
    public void Embeddings_Throws_When_ApiKey_Missing()
    {
        var http = new HttpClient(new HttpClientHandler());
        var cfg = new OpenAISettings
        {
            Endpoint = "https://example.openai.azure.com",
            ApiVersion = "2024-06-01",
            ChatDeployment = "gpt",
            EmbeddingDeployment = "embed",
            ApiKey = string.Empty // missing
        };

        Assert.Throws<InvalidOperationException>(() => new AzureOpenAIEmbeddingService(http, cfg));
    }
}