using CosmicWorks.Configuration;
using CosmicWorks.Infrastructure.Integrations.OpenAI;
using CosmicWorks.Tests.Testing.Infrastructure;
using FluentAssertions;
using System.Net;
using System.Text.Json;
using Xunit;

namespace CosmicWorks.Tests.Infrastructure.Integrations.OpenAI;

public class AzureOpenAIEmbeddingServiceEdgeTests
{
    [Fact]
    public async Task Returns_Empty_Vector_When_Response_Has_No_Data()
    {
        var handler = new StubHttpMessageHandler(_ =>
        {
            var body = JsonSerializer.Serialize(new
            {
                data = Array.Empty<object>()
            });
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(body)
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

        var vec = await svc.EmbedAsync("anything");
        vec.Should().NotBeNull();
        vec.Should().BeEmpty();
    }
}