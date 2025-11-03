using CosmicWorks.Configuration;
using CosmicWorks.Infrastructure.Integrations.OpenAI;
using CosmicWorks.Tests.Testing.Infrastructure;
using FluentAssertions;
using System.Net;
using System.Text.Json;
using Xunit;

namespace CosmicWorks.Tests.Infrastructure.Integrations.OpenAI;

public class AzureOpenAIChatServiceTests
{
    [Fact]
    public async Task Sends_Messages_And_Reads_String_Content()
    {
        var handler = new StubHttpMessageHandler(req =>
        {
            var json = JsonSerializer.Serialize(new
            {
                choices = new[] { new { message = new { role = "assistant", content = "hello!" } } }
            });
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
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

        reply.Should().Be("hello!");
    }
}