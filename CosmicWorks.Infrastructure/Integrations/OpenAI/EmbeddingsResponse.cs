using Newtonsoft.Json;

namespace CosmicWorks.Infrastructure.Integrations.OpenAI;

/// <summary>
/// Wire shape for the Embeddings response (list of vectors).
/// </summary>
internal sealed class EmbeddingsResponse
{
    [JsonProperty("data")]
    public IReadOnlyList<EmbeddingWrapper> Data { get; set; } = Array.Empty<EmbeddingWrapper>();
}