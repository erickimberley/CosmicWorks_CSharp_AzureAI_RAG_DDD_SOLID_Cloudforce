using CosmicWorks.CLI;

namespace CosmicWorks.Tests.Testing.CLI;

internal sealed class ThrowingRouter : ICommandRouter
{
    public Task<string?> RouteAsync(string input, CancellationToken ct = default)
        => throw new InvalidOperationException("boom");
}