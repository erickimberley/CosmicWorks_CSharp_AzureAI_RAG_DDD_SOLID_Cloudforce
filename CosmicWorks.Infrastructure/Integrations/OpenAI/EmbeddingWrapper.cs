using Newtonsoft.Json;

namespace CosmicWorks.Infrastructure.Integrations.OpenAI;

/// <summary>
/// Wrapper used by the Embeddings API around the embedding array.
/// </summary>
internal sealed class EmbeddingWrapper
{
    [JsonProperty("embedding")]
    public IReadOnlyList<float> Embedding { get; set; } = Array.Empty<float>();
}