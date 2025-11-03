namespace CosmicWorks.Application.Abstractions;

/// <summary>
/// Distinct category names known to the system
/// </summary>
public interface ICategoryQueries
{
    Task<IReadOnlyList<string>> ListCategoriesAsync(CancellationToken ct = default);
}