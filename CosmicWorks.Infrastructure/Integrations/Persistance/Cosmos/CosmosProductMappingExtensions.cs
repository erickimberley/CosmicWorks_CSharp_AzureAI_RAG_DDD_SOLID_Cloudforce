using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.ValueObjects;

namespace CosmicWorks.Infrastructure.Integrations.Persistance.Cosmos;

/// <summary>
/// Converters between the Cosmos DB document shape and the domain model.
/// </summary>
internal static class CosmosProductMappingExtensions
{
    // Document -> Domain
    internal static Product ToDomain(this CosmosProductDocument d)
    {
        var p = new Product(
            new ProductId(d.Id),
            new CategoryId(d.CategoryId),
            d.CategoryName ?? string.Empty,
            d.Name ?? string.Empty,
            d.Sku ?? string.Empty,
            d.Description ?? string.Empty,
            new Money((decimal)d.Price));

        if (d.Discount > 0)
            p.ApplyDiscount(DiscountRate.Create(d.Discount));

        return p;
    }

    // Vector row -> Domain
    internal static Product ToDomain(this ProductSimilarityRow r)
    {
        var p = new Product(
            new ProductId(r.Id),
            new CategoryId(r.CategoryId),
            r.CategoryName ?? string.Empty,
            r.Name ?? string.Empty,
            r.Sku ?? string.Empty,
            r.Description ?? string.Empty,
            new Money((decimal)r.Price));

        if (r.Discount > 0)
            p.ApplyDiscount(DiscountRate.Create(r.Discount));

        return p;
    }

    // Domain -> Document
    internal static CosmosProductDocument ToDocument(this Product p, string? etag = null) => new CosmosProductDocument
    {
        Id = p.Id.Value,
        CategoryId = p.CategoryId.Value,
        CategoryName = p.CategoryName,
        Sku = p.Sku,
        Name = p.Name,
        Description = p.Description,
        Price = (double)p.Price.Amount,
        Discount = p.Discount.Value,
        SalePrice = (double)p.SalePrice.Amount,
        ETag = etag
    };
}