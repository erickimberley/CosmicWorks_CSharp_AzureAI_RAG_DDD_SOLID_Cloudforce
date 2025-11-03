using CosmicWorks.Application.UseCases;
using CosmicWorks.Tests.Testing.Application;
using FluentAssertions;
using Xunit;

namespace CosmicWorks.Tests.Application.UseCases;

public class ChatWithCatalogEmptyNeighborsTests
{
    [Fact]
    public async Task Builds_System_With_Zero_Context_And_Still_Calls_Chat()
    {
        var chat = new CaptureChat();
        var uc = new ChatWithCatalog(new EmptySearch(), new NoopEmbed(), chat, new Prompts(), topK: 3, minSimilarity: 0.5);

        var res = await uc.ExecuteAsync("hello");
        res.Should().Be("OK");
        chat.Last![0].content.Should().StartWith("SYS|0"); // explicitly covers the 0-neighbor path
    }
}