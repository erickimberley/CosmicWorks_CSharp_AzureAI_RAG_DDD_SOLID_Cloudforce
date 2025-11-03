using Newtonsoft.Json;

namespace CosmicWorks.Infrastructure.Integrations.OpenAI;

/// <summary>
/// Wire shape for the Chat Completions response (choices only).
/// </summary>
internal sealed class ChatResponse
{
    [JsonProperty("choices")]
    public List<ChatChoice> Choices { get; set; } = new();
}