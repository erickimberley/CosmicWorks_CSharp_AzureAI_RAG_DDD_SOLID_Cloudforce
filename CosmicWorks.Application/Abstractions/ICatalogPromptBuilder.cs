namespace CosmicWorks.Application.Abstractions;

/// <summary>
/// Facade that unifies base system prompt text and context building
/// into a single, easy-to-use component for chat.
/// </summary>
public interface ICatalogPromptBuilder : ISystemPromptTemplate, ICatalogContextPrompt
{    
}