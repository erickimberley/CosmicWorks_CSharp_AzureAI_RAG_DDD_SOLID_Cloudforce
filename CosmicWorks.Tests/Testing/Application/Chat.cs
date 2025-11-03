using CosmicWorks.Application.Abstractions;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class Chat : IChatService
{
    public IReadOnlyList<(string role, string content)>? Last;
    
    public Task<string> ChatAsync(IReadOnlyList<(string role, string content)> messages, CancellationToken ct = default)
    { Last = messages; return Task.FromResult("OK"); }
}