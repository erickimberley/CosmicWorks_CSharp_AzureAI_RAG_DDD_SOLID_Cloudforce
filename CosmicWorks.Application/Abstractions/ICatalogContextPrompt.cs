using CosmicWorks.Domain.Entities;

namespace CosmicWorks.Application.Abstractions;

/// <summary>
/// Builds the catalog-specific system prompt and inline context
/// </summary>
public interface ICatalogContextPrompt
{
    string BuildSystemPrompt(IReadOnlyList<Product> items);
}