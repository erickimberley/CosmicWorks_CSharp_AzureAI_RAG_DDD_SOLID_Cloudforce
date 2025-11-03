using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Repositories;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class NoopSearch : IProductSimilaritySearcher
{
    public Task<IReadOnlyList<Product>> FindSimilarAsync(float[] v, int k, double m, CancellationToken ct = default)
        => Task.FromResult((IReadOnlyList<Product>)Array.Empty<Product>());
}