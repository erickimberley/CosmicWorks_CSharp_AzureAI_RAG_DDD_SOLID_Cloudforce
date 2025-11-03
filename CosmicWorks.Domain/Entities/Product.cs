using CosmicWorks.Domain.Common;
using CosmicWorks.Domain.Events;
using CosmicWorks.Domain.ValueObjects;

namespace CosmicWorks.Domain.Entities;

/// <summary>
/// Domain model for a catalog product. 
/// </summary>
public sealed class Product : Entity
{
    public ProductId Id { get; }

    public CategoryId CategoryId { get; private set; }

    public string CategoryName { get; private set; }

    public string Name { get; private set; }

    public string Sku { get; private set; }

    public string Description { get; private set; }

    public Money Price { get; private set; }

    public DiscountRate Discount { get; private set; } = DiscountRate.Zero;

    public Money SalePrice => Price.Apply(Discount);

    public Product(
        ProductId id,
        CategoryId categoryId,
        string categoryName,
        string name,
        string sku,
        string description,
        Money price)
    {
        Id = id;
        CategoryId = categoryId;
        CategoryName = categoryName ?? string.Empty;
        Name = name ?? string.Empty;
        Sku = sku ?? string.Empty;
        Description = description ?? string.Empty;
        Price = price;
        Discount = DiscountRate.Zero;
    }

    public void ApplyDiscount(DiscountRate rate)
    {
        if (rate.Value < 0 || rate.Value >= 1) throw new ArgumentOutOfRangeException(nameof(rate));
        if (Math.Abs(rate.Value - Discount.Value) < double.Epsilon) return;

        var old = Discount;
        Discount = rate;
        Raise(new DiscountApplied(Id, old, rate));
    }

    public void RemoveDiscount()
    {
        if (Discount.Value <= 0) return;

        var old = Discount;
        Discount = DiscountRate.Zero;
        Raise(new DiscountRemoved(Id, old));
    }
}