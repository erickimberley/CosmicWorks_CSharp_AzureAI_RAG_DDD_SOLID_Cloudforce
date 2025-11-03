using CosmicWorks.CLI;
using CosmicWorks.Tests.Testing.CLI;
using FluentAssertions;
using System.Text.RegularExpressions;
using Xunit;

namespace CosmicWorks.Tests.CLI;

public class ConsoleShell_DiscountAndRemoveTests
{
    private static string Normalize(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return string.Empty;
        var t = s.Replace('\u201C', '"').Replace('\u201D', '"')
                 .Replace('\u2018', '\'').Replace('\u2019', '\'');
        t = Regex.Replace(t, @"\s+", " ").Trim();
        return t.ToLowerInvariant();
    }
    
    [Fact]
    public async Task Shell_Runs_Discount_Command_And_Prints_Result()
    {
        var input = new StringReader($"discount helmets 30%{Environment.NewLine}quit{Environment.NewLine}");
        var output = new StringWriter();

        // Save & swap console streams
        var prevIn = Console.In;
        var prevOut = Console.Out;
        Console.SetIn(input);
        Console.SetOut(output);
                
        var shell = new ConsoleShell(new CommandRouter(new FakeCopilot()));
        await shell.RunAsync();
        
        var text = Normalize(output.ToString());
        text.Should().Contain("applied discount");
        text.Should().MatchRegex(@"applied discount\s+30\s*%");
        text.Should().Contain("helmets");
        text.Should().Contain("updated: 5");

        Console.SetIn(prevIn);
        Console.SetOut(prevOut);     
        input.Dispose();
        output.Dispose();
    }    
}