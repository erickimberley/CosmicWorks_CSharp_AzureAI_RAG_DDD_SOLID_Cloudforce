using CosmicWorks.Domain.Common;
using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Repositories;
using Microsoft.Azure.Cosmos;
using System.Collections.Concurrent;
using System.Net;

namespace CosmicWorks.Infrastructure.Integrations.Persistance.Cosmos;

/// <summary>
/// Cosmos DB implementation of CosmicWorks.Domain.Repositories.IProductRepository.
/// Supports read queries, category listing, and optimistic-concurrency updates.
/// </summary>
internal sealed class CosmosProductRepository : IProductRepository
{
    private readonly Container _container;
    private readonly string _partitionKeyPath; 
    private readonly ConcurrentDictionary<string, string> _etagCache = new();

    public CosmosProductRepository(Container container, string partitionKeyPath)
    {
        _container = container;
        _partitionKeyPath = partitionKeyPath;
    }

    public async Task<IReadOnlyList<Product>> FindByCategoryAsync(string categoryLike, CancellationToken ct = default)
    {
        var q = new QueryDefinition(
            "SELECT * FROM Products p WHERE @cat = '' OR CONTAINS(LOWER(p.category_name), LOWER(@cat))")
            .WithParameter("@cat", categoryLike ?? string.Empty);

        var it = _container.GetItemQueryIterator<CosmosProductDocument>(q);
        var list = new List<Product>();
        while (it.HasMoreResults)
        {
            var page = await it.ReadNextAsync(ct);
            foreach (var d in page)
            {
                if (!string.IsNullOrWhiteSpace(d.ETag)) _etagCache[d.Id] = d.ETag!;                
                list.Add(d.ToDomain());
            }
        }
        return list;
    }
    
    public async Task SaveAsync(Product p, CancellationToken ct = default)
    {
        var id = p.Id.Value;        
        var pkValue = _partitionKeyPath == "/id" ? p.Id.Value : p.CategoryId.Value;

        var ops = new List<PatchOperation>
        {
            PatchOperation.Set("/category_name", p.CategoryName ?? string.Empty),
            PatchOperation.Set("/sku",            p.Sku),
            PatchOperation.Set("/name",           p.Name),
            PatchOperation.Set("/description",    p.Description ?? string.Empty),        
            PatchOperation.Set("/price",    (double)p.Price.Amount),
            PatchOperation.Set("/discount", p.Discount.Value),

            // keep sale_price consistent with discount (null when no discount)
            PatchOperation.Set("/sale_price",
                p.Discount.Value > 0 ? (double?)p.SalePrice.Amount : null)
        };

        _etagCache.TryGetValue(id, out var etag);
        PatchItemRequestOptions? options = string.IsNullOrEmpty(etag) ? null : new PatchItemRequestOptions { IfMatchEtag = etag };

        try
        {
            var resp = await _container.PatchItemAsync<CosmosProductDocument>(
                id,
                new PartitionKey(pkValue),
                ops,
                options,
                ct);

            if (!string.IsNullOrEmpty(resp.ETag))
                _etagCache[id] = resp.ETag;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.PreconditionFailed)
        {
            throw new ConcurrencyException($"Concurrency conflict updating product '{id}'. Please retry.", ex);
        }
    }

    public async Task<IReadOnlyList<string>> ListCategoriesAsync(CancellationToken ct = default)
    {
        var sql = new QueryDefinition("SELECT DISTINCT VALUE c.category_name FROM c");
        var it = _container.GetItemQueryIterator<string>(sql);

        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        while (it.HasMoreResults)
        {
            var page = await it.ReadNextAsync(ct);
            foreach (var cat in page)
                if (!string.IsNullOrWhiteSpace(cat))
                    set.Add(cat);
        }
        return set.OrderBy(c => c).ToList();
    }
}