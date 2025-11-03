using Newtonsoft.Json;

namespace CosmicWorks.Infrastructure.Integrations.OpenAI;

/// <summary>
/// Wire shape for the Embeddings request body.
/// </summary>
internal sealed class EmbeddingsRequest
{
    [JsonProperty("input")]
    public string Input { get; set; } = default!;
}