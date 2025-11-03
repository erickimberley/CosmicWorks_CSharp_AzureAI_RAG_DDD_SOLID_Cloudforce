using CosmicWorks.CLI;
using CosmicWorks.Tests.Testing.CLI;
using FluentAssertions;
using Xunit;

namespace CosmicWorks.Tests.CLI;

public class ConsoleShellTests
{
    [Fact]
    public async Task Prints_Welcome_And_Help_Then_Quits()
    {
        // Simulate typing "help" then "quit"
        using var input = new StringReader($"help{Environment.NewLine}quit{Environment.NewLine}");
        using var output = new StringWriter();
        Console.SetIn(input);
        Console.SetOut(output);

        var shell = new ConsoleShell(new FakeRouter());
        await shell.RunAsync();

        var text = output.ToString();
        text.Should().Contain("CosmicWorks CLI");
        text.Should().Contain("Type 'help' for commands");
        text.Should().Contain("Commands:"); // help text
    }
}