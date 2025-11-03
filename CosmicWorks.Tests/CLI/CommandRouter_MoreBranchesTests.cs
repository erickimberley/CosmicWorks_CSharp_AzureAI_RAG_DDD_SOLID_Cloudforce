using CosmicWorks.CLI;
using CosmicWorks.Tests.Testing.CLI;
using FluentAssertions;
using Xunit;

namespace CosmicWorks.Tests.CLI;

public class CommandRouter_MoreBranchesTests
{
    [Fact]
    public async Task Discount_Integer_WithoutPercent_Treated_As_Percent()
    {
        var r = new CommandRouter(new NoopCopilot());
        var msg = await r.RouteAsync("discount helmets 30");        
        msg.Should().NotBeNull();
        msg!.Should().ContainEquivalentOf("applied discount 30");
    }

    [Fact]
    public void Discount_Invalid_Rate_Throws_FormatException()
    {
        var r = new CommandRouter(new NoopCopilot());
        var act = () => r.RouteAsync("discount helmets notanumber");
        act.Should().ThrowAsync<FormatException>();
    }
}