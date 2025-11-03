using CosmicWorks.Application.UseCases;
using CosmicWorks.Tests.Testing.Application;
using FluentAssertions;
using Xunit;

namespace CosmicWorks.Tests.Application.UseCases;

public class ChatWithCatalogTests
{
    [Fact]
    public async Task Wires_Embedding_Search_Prompt_And_Chat_In_Order()
    {
        var search = new FakeSearch();
        var embed = new FakeEmbed();
        var chat = new CapturingChat();
        var prompts = new FakePrompts();

        var usecase = new ChatWithCatalog(search, embed, chat, prompts, topK: 4, minSimilarity: 0.2);

        var result = await usecase.ExecuteAsync("show helmets");

        result.Should().Be("ok");
        embed.LastText.Should().Be("show helmets");
        search.LastVector.Should().NotBeNull();
        search.LastK.Should().Be(4);
        search.LastMin.Should().Be(0.2);

        chat.Last![0].role.Should().Be("system");
        chat.Last![0].content.Should().StartWith("SYS|1"); // one neighbor
        chat.Last![1].content.Should().Be("show helmets");
    }
}