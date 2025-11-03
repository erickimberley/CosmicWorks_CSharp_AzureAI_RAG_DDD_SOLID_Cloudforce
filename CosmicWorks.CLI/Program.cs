using CosmicWorks.CLI;
using CosmicWorks.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Application entry point for the CosmicWorks CLI.
/// </summary>
public static class Program
{
    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables("Cosmic_Works_")
        .Build();

        var services = new ServiceCollection();        
        services.AddCosmicWorksInfrastructure(configuration).AddCosmicWorksCli();
                
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        });

        // hand off to the console shell
        await provider.GetRequiredService<IConsoleShell>().RunAsync();
    }
}