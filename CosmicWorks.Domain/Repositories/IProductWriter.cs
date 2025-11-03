using CosmicWorks.Domain.Entities;

namespace CosmicWorks.Domain.Repositories;

/// <summary>
/// Write-side abstraction for persisting product aggregates.
/// </summary>
public interface IProductWriter
{
    Task SaveAsync(Product product, CancellationToken ct = default);
}