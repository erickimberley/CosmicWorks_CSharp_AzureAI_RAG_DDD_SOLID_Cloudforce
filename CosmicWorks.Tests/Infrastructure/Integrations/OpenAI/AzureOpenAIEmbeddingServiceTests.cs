using CosmicWorks.Configuration;
using CosmicWorks.Infrastructure.Integrations.OpenAI;
using CosmicWorks.Tests.Testing.Infrastructure;
using FluentAssertions;
using System.Net;
using System.Text.Json;
using Xunit;

namespace CosmicWorks.Tests.Infrastructure.Integrations.OpenAI;

public class AzureOpenAIEmbeddingServiceTests
{
    [Fact]
    public async Task Posts_Correct_URL_And_Parses_Response()
    {
        var handler = new StubHttpMessageHandler(req =>
        {
            req.RequestUri!.AbsoluteUri.Should().Contain("/openai/deployments/embeddings/embeddings");
            req.Headers.Should().Contain(h => h.Key == "api-key");
            var json = JsonSerializer.Serialize(new
            {
                data = new[] { new { embedding = new[] { 0.1, 0.2, 0.3 } } }
            });
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
        });

        var http = new HttpClient(handler);
        var svc = new AzureOpenAIEmbeddingService(http, new OpenAISettings
        {
            Endpoint = "https://example.openai.azure.com",
            ApiVersion = "2024-06-01",
            EmbeddingDeployment = "embeddings",
            ApiKey = "test-key"
        });

        var vec = await svc.EmbedAsync("hi");

        vec.Should().BeEquivalentTo(new float[] { 0.1f, 0.2f, 0.3f });
    }

    [Fact]
    public async Task Throws_On_Non_Success_Status_Code()
    {
        var handler = new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.BadRequest));

        var http = new HttpClient(handler);
        var svc = new AzureOpenAIEmbeddingService(http, new OpenAISettings
        {
            Endpoint = "https://example.openai.azure.com",
            ApiVersion = "2024-06-01",
            EmbeddingDeployment = "embeddings",
            ApiKey = "test-key"
        });

        await Assert.ThrowsAsync<HttpRequestException>(() => svc.EmbedAsync("boom"));
    }
}