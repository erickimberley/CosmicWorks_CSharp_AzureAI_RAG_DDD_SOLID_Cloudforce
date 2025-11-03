using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Repositories;
using Microsoft.Azure.Cosmos;

namespace CosmicWorks.Infrastructure.Integrations.Persistance.Cosmos;

/// <summary>
/// Vector search against Cosmos DB using the VectorDistance query pattern
/// to retrieve semantically similar products.
/// </summary>
internal sealed class CosmosProductSimilaritySearcher : IProductSimilaritySearcher
{
    private readonly Container _container;

    public CosmosProductSimilaritySearcher(Container container) => _container = container;

    public async Task<IReadOnlyList<Product>> FindSimilarAsync(float[] vector, int topK, double minSim, CancellationToken ct = default)
    {
        var sql = @"
            SELECT TOP @k
              p.id, p.category_id, p.category_name, p.sku, p.name, p.description, p.price, p.discount, p.sale_price,
              p._etag AS _etag,
              VectorDistance(p.embedding, @q) AS similarity_score
            FROM Products p
            WHERE VectorDistance(p.embedding, @q) > @min
            ORDER BY VectorDistance(p.embedding, @q)";

        var q = new QueryDefinition(sql)
            .WithParameter("@k", topK)
            .WithParameter("@q", vector)
            .WithParameter("@min", minSim);

        var it = _container.GetItemQueryIterator<ProductSimilarityRow>(q);
        var list = new List<Product>();

        while (it.HasMoreResults)
        {
            var page = await it.ReadNextAsync(ct);
            foreach (var r in page)
                list.Add(r.ToDomain());
        }

        return list;
    }
}