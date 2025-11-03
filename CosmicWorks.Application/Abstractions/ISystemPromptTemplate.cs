namespace CosmicWorks.Application.Abstractions;

/// <summary>
/// Provides the static base system prompt (without inline product context).
/// </summary>
public interface ISystemPromptTemplate
{
    string BuildBasePrompt();
}