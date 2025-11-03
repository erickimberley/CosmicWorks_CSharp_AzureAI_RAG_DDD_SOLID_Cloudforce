using CosmicWorks.CLI;
using CosmicWorks.Tests.Testing.CLI;
using FluentAssertions;
using System.Text.RegularExpressions;
using Xunit;

namespace CosmicWorks.Tests.CLI;

public class ConsoleShell_ExtraBranchesTests
{    
    private static string Squash(string s) => Regex.Replace(s, @"\s+", " ").Trim().ToLowerInvariant();

    [Fact]
    public async Task Help_And_QuestionMark_Print_Usage()
    {
        var prevIn = Console.In; var prevOut = Console.Out;
        var input = new StringReader($"help{Environment.NewLine}?{Environment.NewLine}quit{Environment.NewLine}");
        var output = new StringWriter();
        Console.SetIn(input); Console.SetOut(output);
        try
        {
            var shell = new ConsoleShell(new CommandRouter(new NoopCopilot()));
            await shell.RunAsync();
        }
        finally { Console.SetIn(prevIn); Console.SetOut(prevOut); input.Dispose(); output.Dispose(); }

        var text = Squash(output.ToString());
        text.Should().Contain("commands:"); // usage banner
        text.Should().Contain("type 'help'"); // welcome banner present too
    }

    [Fact]
    public async Task Exit_Alias_Breaks_Loop()
    {
        var prevIn = Console.In; var prevOut = Console.Out;
        var input = new StringReader($"exit{Environment.NewLine}");
        var output = new StringWriter();
        Console.SetIn(input); Console.SetOut(output);
        try
        {
            var shell = new ConsoleShell(new CommandRouter(new NoopCopilot()));
            await shell.RunAsync();
        }
        finally { Console.SetIn(prevIn); Console.SetOut(prevOut); input.Dispose(); output.Dispose(); }

        Squash(output.ToString()).Should().Contain("cosmicworks cli");
    }

    [Fact]
    public async Task Error_Path_Prints_Red_Message_Then_Continues()
    {
        // A router that always throws to force the shell's catch/log path
        var throwing = new ThrowingRouter();

        var prevIn = Console.In;
        var prevOut = Console.Out;
        var input = new StringReader($"boom{Environment.NewLine}quit{Environment.NewLine}");
        var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        try
        {
            var shell = new ConsoleShell(throwing);
            await shell.RunAsync();
        }
        finally
        {
            Console.SetIn(prevIn);
            Console.SetOut(prevOut);
            input.Dispose();
            output.Dispose();
        }

        var text = Regex.Replace(output.ToString(), @"\s+", " ").Trim().ToLowerInvariant();

        // Shell should catch and print an error message, including our exception text "boom"        
        text.Should().Contain("boom");
    }    
}