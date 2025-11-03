using CosmicWorks.Application.Abstractions;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class CaptureChat : IChatService
{
    public IReadOnlyList<(string role, string content)>? Last;
    public Task<string> ChatAsync(IReadOnlyList<(string role, string content)> m, CancellationToken ct = default)
    { 
        Last = m; return Task.FromResult("OK"); 
    }
}