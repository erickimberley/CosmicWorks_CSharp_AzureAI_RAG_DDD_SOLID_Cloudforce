using CosmicWorks.Application.Abstractions;
using CosmicWorks.Domain.Entities;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class FakePrompts : ICatalogPromptBuilder
{
    public string BuildBasePrompt() => "BASE";
    public string BuildSystemPrompt(IReadOnlyList<Product> items) => "SYS|" + items.Count;
}