using CosmicWorks.Domain.Entities;

namespace CosmicWorks.Domain.Repositories;

/// <summary>
/// Vector-search abstraction for retrieving semantically similar products.
/// </summary>
public interface IProductSimilaritySearcher
{
    Task<IReadOnlyList<Product>> FindSimilarAsync(float[] vector, int topK, double minSim, CancellationToken ct = default);
}