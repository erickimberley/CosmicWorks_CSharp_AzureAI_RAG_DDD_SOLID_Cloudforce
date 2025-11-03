using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Repositories;
using CosmicWorks.Domain.ValueObjects;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class FakeSearch : IProductSimilaritySearcher
{
    public float[]? LastVector;
    public int LastK;
    public double LastMin;

    public Task<IReadOnlyList<Product>> FindSimilarAsync(
        float[] vector, int topK, double minSim, CancellationToken ct = default)
    {
        LastVector = vector; LastK = topK; LastMin = minSim;
        var items = new List<Product>
        {
            new Product(new ProductId("p1"), new CategoryId("c"),
                        "Accessories, Helmets","Helmet","HL-1","desc", new Money(100m))
        };
        return Task.FromResult((IReadOnlyList<Product>)items);
    }
}