using CosmicWorks.Application.Abstractions;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class Embed : IEmbeddingService
{
    public string? Last; 
    
    public Task<float[]> EmbedAsync(string t, CancellationToken ct = default) { Last = t; return Task.FromResult(new float[] { 0.1f }); }
}