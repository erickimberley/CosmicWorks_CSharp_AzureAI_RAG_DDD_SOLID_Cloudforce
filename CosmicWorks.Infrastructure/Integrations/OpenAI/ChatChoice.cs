using Newtonsoft.Json;

namespace CosmicWorks.Infrastructure.Integrations.OpenAI;

/// <summary>
/// DTO for a single choice returned by Chat Completions.
/// </summary>
internal sealed class ChatChoice
{
    [JsonProperty("message")]
    public ChatMessage Message { get; set; } = default!;
}