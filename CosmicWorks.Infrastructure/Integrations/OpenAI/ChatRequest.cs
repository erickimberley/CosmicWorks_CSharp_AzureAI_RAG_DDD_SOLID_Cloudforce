using Newtonsoft.Json;

namespace CosmicWorks.Infrastructure.Integrations.OpenAI;

/// <summary>
/// Wire shape for the Chat Completions request.
/// </summary>
internal sealed class ChatRequest
{
    [JsonProperty("messages")]
    public List<ChatMessage> Messages { get; set; } = new();

    [JsonProperty("temperature")]
    public double Temperature { get; set; } = 0.2;
}