using CosmicWorks.Configuration;
using CosmicWorks.Infrastructure.Integrations.OpenAI;
using CosmicWorks.Tests.Testing.Infrastructure;
using FluentAssertions;
using System.Net;
using System.Text.Json;
using Xunit;

namespace CosmicWorks.Tests.Infrastructure.Integrations.OpenAI;

public class AzureOpenAIChatServiceEdgeTests
{
    [Fact]
    public async Task Returns_No_Content_When_Response_Has_No_Choices()
    {
        var handler = new StubHttpMessageHandler(_ =>
        {
            var body = JsonSerializer.Serialize(new
            {
                choices = Array.Empty<object>() // no choices provided by API
            });

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(body)
            };
        });

        var http = new HttpClient(handler);
        var svc = new AzureOpenAIChatService(http, new OpenAISettings
        {
            Endpoint = "https://example.openai.azure.com",
            ApiVersion = "2024-06-01",
            ChatDeployment = "gpt",
            ApiKey = "test-key"
        });

        var reply = await svc.ChatAsync(new List<(string role, string content)>
        {
            ("system","you are helpful"),
            ("user","say hi")
        });

        reply.Should().Be("(no content)");
    }

    [Fact]
    public async Task Throws_When_Api_Returns_Unauthorized()
    {
        var handler = new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.Unauthorized));

        var http = new HttpClient(handler);
        var svc = new AzureOpenAIChatService(http, new OpenAISettings
        {
            Endpoint = "https://example.openai.azure.com",
            ApiVersion = "2024-06-01",
            ChatDeployment = "gpt",
            ApiKey = "bad-key"
        });

        await Assert.ThrowsAsync<HttpRequestException>(() => svc.ChatAsync(new List<(string, string)>
        {
            ("user","ping")
        }));
    }
}