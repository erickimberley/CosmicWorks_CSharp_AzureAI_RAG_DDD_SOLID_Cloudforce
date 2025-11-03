using CosmicWorks.Domain.Entities;

namespace CosmicWorks.Domain.Repositories;

/// <summary>
/// Read-only operations for the product catalog.
/// </summary>
public interface IProductReader
{
    Task<IReadOnlyList<Product>> FindByCategoryAsync(string categoryLike, CancellationToken ct = default);

    Task<IReadOnlyList<string>> ListCategoriesAsync(CancellationToken ct = default);
}