using CosmicWorks.Application.Abstractions;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class NoopChat : IChatService 
{ 
    public Task<string> ChatAsync(IReadOnlyList<(string role, string content)> m, CancellationToken ct = default) => Task.FromResult(string.Empty); 
}