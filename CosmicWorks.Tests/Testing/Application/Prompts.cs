using CosmicWorks.Application.Abstractions;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class Prompts : ICatalogPromptBuilder
{
    public string BuildBasePrompt() => "BASE";
    public string BuildSystemPrompt(IReadOnlyList<CosmicWorks.Domain.Entities.Product> items) => "SYS|" + items.Count;
}