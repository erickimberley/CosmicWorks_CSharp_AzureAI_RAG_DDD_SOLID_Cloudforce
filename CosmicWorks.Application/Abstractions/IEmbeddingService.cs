namespace CosmicWorks.Application.Abstractions;

/// <summary>
/// Provides text embeddings used for vector similarity search.
/// </summary>
public interface IEmbeddingService
{
    Task<float[]> EmbedAsync(string text, CancellationToken ct = default);
}