namespace CosmicWorks.CLI;

/// <summary>
/// Owns the console Read–Eval–Print Loop. Routes lines to ICommandRouter.
/// </summary>
internal interface IConsoleShell
{
    Task RunAsync(CancellationToken ct = default);
}