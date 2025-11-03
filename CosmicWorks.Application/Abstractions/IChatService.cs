namespace CosmicWorks.Application.Abstractions;

/// <summary>
/// Chat client abstraction (e.g., Azure OpenAI).
/// Takes pre-baked role-tagged messages and returns a single response.
/// </summary>
public interface IChatService
{
    Task<string> ChatAsync(IReadOnlyList<(string role, string content)> messages, CancellationToken ct = default);
}