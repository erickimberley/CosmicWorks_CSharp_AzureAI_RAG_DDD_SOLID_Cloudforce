using CosmicWorks.Application.Abstractions;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class CapturingChat : IChatService
{
    public List<(string role, string content)>? Last;
    public Task<string> ChatAsync(IReadOnlyList<(string role, string content)> messages, CancellationToken ct = default)
    { 
        Last = messages.ToList(); return Task.FromResult("ok"); 
    }
}