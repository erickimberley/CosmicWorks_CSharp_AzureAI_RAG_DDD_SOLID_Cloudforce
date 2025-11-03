using CosmicWorks.CLI;

namespace CosmicWorks.Tests.Testing.CLI;

internal sealed class FakeRouter : ICommandRouter
{
    public Task<string?> RouteAsync(string input, CancellationToken ct = default)
        => Task.FromResult<string?>($"ROUTE:{input}");
}