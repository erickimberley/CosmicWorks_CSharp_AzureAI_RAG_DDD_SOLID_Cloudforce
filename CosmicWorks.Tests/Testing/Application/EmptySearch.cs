using CosmicWorks.Domain.Repositories;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class EmptySearch : IProductSimilaritySearcher
{
    public Task<IReadOnlyList<CosmicWorks.Domain.Entities.Product>> FindSimilarAsync(float[] v, int k, double m, CancellationToken ct = default)
        => Task.FromResult((IReadOnlyList<CosmicWorks.Domain.Entities.Product>)Array.Empty<CosmicWorks.Domain.Entities.Product>());
}