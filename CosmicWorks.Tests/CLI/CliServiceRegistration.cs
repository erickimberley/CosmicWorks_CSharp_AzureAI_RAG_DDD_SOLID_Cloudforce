using CosmicWorks.Application.Abstractions;
using CosmicWorks.CLI;
using CosmicWorks.Tests.Testing.CLI;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CosmicWorks.Tests.CLI;

public class CliServiceRegistration_CoverageTests
{    
    [Fact]
    public void AddCosmicWorksCli_Registers_Router_And_Shell()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ICopilotService, FakeCLICopilot>();
        services.AddCosmicWorksCli();

        var sp = services.BuildServiceProvider();

        sp.GetRequiredService<ICommandRouter>().Should().NotBeNull();
        sp.GetRequiredService<IConsoleShell>().Should().NotBeNull();
    }
}