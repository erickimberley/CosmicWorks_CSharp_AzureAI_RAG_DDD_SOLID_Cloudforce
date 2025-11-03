using CosmicWorks.Application.Abstractions;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class FakeEmbed : IEmbeddingService
{
    public string? LastText;
    public Task<float[]> EmbedAsync(string text, CancellationToken ct = default)
    { LastText = text; return Task.FromResult(new float[] { 0.1f, 0.2f }); }
}