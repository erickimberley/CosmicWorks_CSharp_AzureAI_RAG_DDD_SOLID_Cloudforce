using Microsoft.Extensions.DependencyInjection;

namespace CosmicWorks.CLI;

/// <summary>
/// DI bootstrap for the CLI surface. Registers the console shell and router.
/// </summary>
public static class CliServiceRegistration
{
    public static IServiceCollection AddCosmicWorksCli(this IServiceCollection services)
    {
        services.AddSingleton<ICommandRouter, CommandRouter>();
        services.AddSingleton<IConsoleShell, ConsoleShell>();
        return services;
    }
}