using CosmicWorks.Application.Abstractions;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class NoopEmbed : IEmbeddingService 
{ 
    public Task<float[]> EmbedAsync(string t, CancellationToken ct = default) => Task.FromResult(Array.Empty<float>()); 
}