namespace CosmicWorks.Application.Abstractions;

/// <summary>
/// High-level chat specialized for the product catalog.
/// </summary>
public interface ICatalogChat
{
    Task<string> ChatAsync(string prompt, CancellationToken ct = default);   
}