using Newtonsoft.Json;

namespace CosmicWorks.Infrastructure.Integrations.OpenAI;

/// <summary>
/// Single chat message in the request/response.
/// </summary>
internal sealed class ChatMessage
{
    [JsonProperty("role")]
    public string Role { get; set; } = default!;

    [JsonProperty("content")]
    public string Content { get; set; } = default!;
}