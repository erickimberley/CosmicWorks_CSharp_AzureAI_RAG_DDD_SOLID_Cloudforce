namespace CosmicWorks.CLI;

/// <summary>
/// Parses a single console line and executes the appropriate action.
/// Returns a printable string (or null to print nothing).
/// </summary>
internal interface ICommandRouter
{
    Task<string?> RouteAsync(string input, CancellationToken ct = default);
}