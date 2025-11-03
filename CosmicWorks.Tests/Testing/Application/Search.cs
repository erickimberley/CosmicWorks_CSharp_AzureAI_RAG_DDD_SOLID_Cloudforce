using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Repositories;
using CosmicWorks.Domain.ValueObjects;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class Search : IProductSimilaritySearcher
{
    public int LastK; public double LastMin; public float[]? LastVector;
    public Task<IReadOnlyList<Product>> FindSimilarAsync(float[] v, int k, double min, CancellationToken ct = default)
    {
        LastVector = v; LastK = k; LastMin = min;
        return Task.FromResult<IReadOnlyList<Product>>(new[]
        {
                new Product(new ProductId("p1"), new CategoryId("c1"), "Bikes", "Road 100", "R-100", "fast", new Money(999m))
            });
    }
}