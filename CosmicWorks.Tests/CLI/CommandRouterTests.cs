using CosmicWorks.Application.Abstractions;
using CosmicWorks.Application.UseCases;
using CosmicWorks.CLI;
using CosmicWorks.Tests.Testing.CLI;
using FluentAssertions;
using Moq;
using System.Text.RegularExpressions;
using Xunit;

namespace CosmicWorks.Tests.CLI;

public class CommandRouterTests
{
    private static string Normalize(string? s)
    {
        if (s is null) return string.Empty;
        // Normalize smart quotes and whitespace; lower for case-insensitive contains
        var t = s.Replace('\u201C', '"')
                 .Replace('\u201D', '"')
                 .Replace('\u2018', '\'')
                 .Replace('\u2019', '\'');
        t = Regex.Replace(t, @"\s+", " ").Trim();
        return t.ToLowerInvariant();
    }
    
    [Fact]
    public async Task Lists_Categories()
    {
        var router = new CommandRouter(new FakeCopilot());
        var text = await router.RouteAsync("list categories");
        text.Should().Be("- Accessories, Helmets" + Environment.NewLine + "- Bikes, Touring");
    }

    [Fact]
    public async Task Shows_System_Prompt()
    {
        var cp = new FakeCopilot { BasePrompt = "SYS|1 neighbor" };
        var router = new CommandRouter(cp);
        (await router.RouteAsync("prompt"))!.Should().Be("SYS|1 neighbor");
    }

    [Fact]
    public async Task Applies_Discount_When_Given_Percent_Text()
    {
        var router = new CommandRouter(new FakeCopilot());
        var msg = await router.RouteAsync(@"discount ""Touring"" 30%");

        var n = Normalize(msg);
        n.Should().Contain("applied discount");
        // accept "30%" or "30 %"
        n.Should().MatchRegex(@"applied discount\s+30\s*%");
        n.Should().Contain("touring");
        n.Should().Contain("updated: 5");
        n.Should().Contain("discount adjusted: 0");
    }

    [Fact]    
    public async Task Applies_Discount_When_Given_Decimal()
    {
        var router = new CommandRouter(new FakeCopilot());
        var msg = await router.RouteAsync(@"discount helmets 0.45"); // clamp to 30%

        var n = Normalize(msg);
        n.Should().MatchRegex(@"applied discount\s+30\s*%"); // 30% applied
        n.Should().Contain("helmets");                       // category echoed
        n.Should().Contain("updated: 5");                    // items affected
        // We just require that the output indicates some adjustment/clamp happened
        n.Should().MatchRegex("(adjusted|clamped|policy)");  // wording tolerant
    }

    [Fact]
    public async Task Removes_Discount_By_Category()
    {
        var router = new CommandRouter(new FakeCopilot());
        var msg = await router.RouteAsync("remove helmets");
        Normalize(msg).Should().Contain("removed discounts").And.Contain("7");
    }

    [Fact]
    public async Task Falls_Back_To_Chat()
    {
        var router = new CommandRouter(new FakeCopilot());
        var reply = await router.RouteAsync("hello there");
        reply.Should().Be("echo:hello there");
    }

    [Fact]
    public async Task Bad_Discount_Rate_Returns_Helpful_Message_Not_Exception()
    {
        var router = new CommandRouter(new FakeCopilot());
        var msg = await router.RouteAsync(@"discount helmets notanumber");
        var n = Normalize(msg);

        // Accept either a helpful validation-style message OR a fallback to chat
        var looksHelpful = Regex.IsMatch(n, "(invalid|could not parse|not a number|usage|help)");
        var fellBackToChat = n.StartsWith("echo:");

        (looksHelpful || fellBackToChat).Should().BeTrue(
            $"CLI may either show a usage/validation message or fall back to chat; actual: {msg}");
    }

    private static (CommandRouter router, Mock<ICopilotService> copilot) Make()
    {
        var copilot = new Mock<ICopilotService>(MockBehavior.Strict);
        return (new CommandRouter(copilot.Object), copilot);
    }

    [Fact]
    public async Task ListCategories_Command()
    {
        var (router, copilot) = Make();
        copilot.Setup(c => c.ListCategoriesAsync(default)).ReturnsAsync(new[] { "A", "B" });
        var output = await router.RouteAsync("list categories");
        output.Should().Contain("- A").And.Contain("- B");
        copilot.VerifyAll();
    }

    [Theory]
    [InlineData("discount helmets 30%", 0.30)]
    [InlineData("discount helmets 30", 0.30)]
    [InlineData("discount helmets 0.25", 0.25)]
    public async Task Discount_Command_ParsesNumbers(string cmd, double expectedRate)
    {
        var (router, copilot) = Make();
        copilot
            .Setup(c => c.ApplyDiscountAsync("helmets", expectedRate, default))
            .ReturnsAsync(new ApplyDiscountResult(UpdatedCount: 3, ClampedCount: 1, RequestedRate: expectedRate, MinAppliedRate: expectedRate, MaxAppliedRate: expectedRate));

        var output = await router.RouteAsync(cmd);
        output.Should().Contain("3").And.Contain("%");
        copilot.VerifyAll();
    }

    [Fact]
    public async Task Remove_Command()
    {
        var (router, copilot) = Make();
        copilot.Setup(c => c.RemoveDiscountAsync("bags", default)).ReturnsAsync(2);
        var output = await router.RouteAsync("remove bags");
        output.Should().Contain("2");
        copilot.VerifyAll();
    }

    [Fact]
    public async Task Prompt_Command()
    {
        var (router, copilot) = Make();
        copilot.Setup(c => c.BuildBasePrompt()).Returns("SYSTEM-PROMPT");
        var output = await router.RouteAsync("prompt");
        output.Should().Contain("SYSTEM-PROMPT");
        copilot.VerifyAll();
    }

    [Fact]
    public async Task Fallback_GoesToChat()
    {
        var (router, copilot) = Make();
        copilot.Setup(c => c.ChatAsync("hello", default)).ReturnsAsync("chat reply");
        var output = await router.RouteAsync("hello");
        output.Should().Be("chat reply");
        copilot.VerifyAll();
    }

    [Theory]
    [InlineData("discount helmets nope")]
    [InlineData("discount helmets 12x")]
    public async Task Discount_BadNumber_FallsBackToChat(string input)
    {
        var (router, copilot) = Make();

        // Router should treat it as normal chat; never try ApplyDiscountAsync
        copilot
            .Setup(c => c.ChatAsync(input, default))
            .ReturnsAsync("fallback");

        var result = await router.RouteAsync(input);

        result.Should().Be("fallback");

        copilot.Verify(c => c.ApplyDiscountAsync(It.IsAny<string>(), It.IsAny<double>(), default), Times.Never);
        copilot.Verify(c => c.ChatAsync(input, default), Times.Once);
        copilot.VerifyNoOtherCalls();
    }
}