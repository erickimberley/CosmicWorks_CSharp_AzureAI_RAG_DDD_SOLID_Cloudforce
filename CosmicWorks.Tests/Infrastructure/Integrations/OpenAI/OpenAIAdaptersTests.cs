using CosmicWorks.Configuration;
using CosmicWorks.Infrastructure.Integrations.OpenAI;
using CosmicWorks.Tests.Testing.Infrastructure;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json;
using Xunit;

namespace CosmicWorks.Tests.Infrastructure.Integrations.OpenAI;

public class OpenAIAdaptersTests
{
    [Fact]
    public async Task EmbeddingService_SendsExpectedRequest_AndParsesResponse()
    {
        var cfg = new OpenAISettings
        {
            Endpoint = "https://example.azure.com",
            ApiKey = "KEY",
            ApiVersion = "2024-06-01",
            EmbeddingDeployment = "embeddings"
        };

        var handler = new StubHandler(_ =>
        {
            var payload = new EmbeddingsResponse
            {
                Data = new[] { new EmbeddingWrapper { Embedding = new float[] { 0.1f, 0.2f } } }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(
                payload,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
        });

        var http = new HttpClient(handler);
        var svc = new AzureOpenAIEmbeddingService(http, cfg);

        var vec = await svc.EmbedAsync("hello");
        vec.Should().BeEquivalentTo(new[] { 0.1f, 0.2f });

        handler.LastRequest!.RequestUri!.ToString()
            .Should().Contain("/openai/deployments/embeddings/embeddings")
            .And.Contain("api-version=2024-06-01");

        handler.LastRequest!.Headers.GetValues("api-key").Single().Should().Be("KEY");
    }

    [Fact]
    public async Task ChatService_SendsExpectedRequest_AndReturnsContent()
    {
        var cfg = new OpenAISettings
        {
            Endpoint = "https://example.azure.com",
            ApiKey = "KEY",
            ApiVersion = "2024-06-01",
            ChatDeployment = "gpt35"
        };

        var handler = new StubHandler(req =>
        {
            var resp = new ChatResponse
            {
                Choices = new()
                {
                    new ChatChoice { Message = new ChatMessage { Role = "assistant", Content = "Hi Eric!" } }
                }
            };
            var json = JsonConvert.SerializeObject(resp);
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
        });

        var http = new HttpClient(handler);
        var svc = new AzureOpenAIChatService(http, cfg);

        // ✅ pass a list of (role, content) tuples + a CancellationToken
        var messages = new List<(string role, string content)>
        {
            ("system", "system here"),
            ("user",   "user here")
        };

        var text = await svc.ChatAsync(messages, CancellationToken.None);
        text.Should().Be("Hi Eric!");

        handler.LastRequest!.RequestUri!.ToString()
            .Should().Contain("/openai/deployments/gpt35/chat/completions")
            .And.Contain("api-version=2024-06-01");

        handler.LastRequest!.Headers.GetValues("api-key").Single().Should().Be("KEY");
    }

    [Fact]
    public void ChatDto_RoundTrips_WithNewtonsoft()
    {
        var req = new ChatRequest
        {
            Temperature = 0.4,
            Messages = new()
            {
                new ChatMessage { Role = "system", Content = "sys" },
                new ChatMessage { Role = "user",   Content = "hi"  }
            }
        };

        var json = JsonConvert.SerializeObject(req);
        json.Should().Contain("\"messages\"");
        json.Should().Contain("\"temperature\"");
        json.Should().Contain("\"role\"");
        json.Should().Contain("\"content\"");

        var resp = new ChatResponse
        {
            Choices = new()
            {
                new ChatChoice { Message = new ChatMessage { Role = "assistant", Content = "hello" } }
            }
        };

        var round = JsonConvert.DeserializeObject<ChatResponse>(JsonConvert.SerializeObject(resp))!;
        round.Choices.Single().Message.Content.Should().Be("hello");
    }
}