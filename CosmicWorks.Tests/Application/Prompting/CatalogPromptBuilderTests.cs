using CosmicWorks.Application.Prompting;
using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CosmicWorks.Tests.Application.Prompting;

public class CatalogPromptBuilderTests
{
    [Fact]
    public void BuildSystemPrompt_Contains_Base_And_Context()
    {
        var pb = new CatalogPromptBuilder();
        var items = new[]
        {
            new Product(new ProductId("p1"), new CategoryId("c"), "Bikes","Road","R-1","fast", new Money(999.99m))
        };
        var sys = pb.BuildSystemPrompt(items);

        sys.Should().Contain("You are an intelligent copilot");
        sys.Should().Contain("Context:");
        sys.Should().Contain("Road | fast | R-1 | 999.99");
    }

    [Fact]
    public void BuildBasePrompt_Stable()
    {
        var pb = new CatalogPromptBuilder();
        pb.BuildBasePrompt().Should().Contain("Cosmic Works");
    }
}