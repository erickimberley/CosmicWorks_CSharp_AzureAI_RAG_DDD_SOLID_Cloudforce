using CosmicWorks.Application.UseCases;
using CosmicWorks.Tests.Testing.Application;
using FluentAssertions;
using Xunit;

namespace CosmicWorks.Tests.Application.UseCases;

public class CopilotServiceTests
{   
    [Fact]
    public void BuildBasePrompt_Is_Delegated_To_PromptBuilder()
    {
        var apply = new ApplyProductDiscount(new NoopRepo(), new NoopDispatcher(), new NoopPolicy());
        var remove = new RemoveProductDiscount(new NoopRepo(), new NoopRepo(), new NoopDispatcher());
        var chat = new ChatWithCatalog(new NoopSearch(), new NoopEmbed(), new NoopChat(), new FakePrompts(), 3, 0.2);
        var list = new ListCategories(new NoopRepo());
        var prompts = new FakePrompts();

        var svc = new CopilotService(apply, remove, chat, list, prompts);
        svc.BuildBasePrompt().Should().Be("BASE");
    }
}