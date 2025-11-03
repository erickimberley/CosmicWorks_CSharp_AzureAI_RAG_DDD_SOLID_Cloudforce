using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Events;
using CosmicWorks.Domain.ValueObjects;
using Xunit;

namespace CosmicWorks.Tests.Domain.Entities;

public class ProductTests
{
    private static Product NewProduct(
        string id = "P1", string catId = "C1", string categoryName = "Bikes",
        string name = "Road Bike", string sku = "RB-1", string desc = "fast", decimal price = 100m)
        => new Product(new ProductId(id), new CategoryId(catId), categoryName, name, sku, desc, new Money(price));

    [Fact]
    public void Ctor_Initializes_Fields_And_Defaults()
    {
        var p = NewProduct(price: 742.35m);

        Assert.Equal("P1", p.Id.Value);
        Assert.Equal("C1", p.CategoryId.Value);
        Assert.Equal("Bikes", p.CategoryName);
        Assert.Equal("Road Bike", p.Name);
        Assert.Equal("RB-1", p.Sku);
        Assert.Equal("fast", p.Description);
        Assert.Equal(742.35m, p.Price.Amount);

        // Defaults
        Assert.Equal(0.0, p.Discount.Value);
        Assert.Equal(p.Price.Amount, p.SalePrice.Amount);
        Assert.Empty(p.DomainEvents);
    }

    [Fact]
    public void ApplyDiscount_Changes_Rate_SalePrice_And_Raises_Event()
    {
        var p = NewProduct(price: 200m);

        p.ApplyDiscount(DiscountRate.Create(0.25));

        Assert.Equal(0.25, p.Discount.Value, 5);
        Assert.Equal(150m, p.SalePrice.Amount);

        var ev = Assert.Single(p.DomainEvents);
        var applied = Assert.IsType<DiscountApplied>(ev);
        Assert.Equal("P1", applied.ProductId.Value);
        Assert.Equal(0.0, applied.OldRate.Value);
        Assert.Equal(0.25, applied.NewRate.Value);

        var dequeued = p.DequeueDomainEvents();   // clears queue after persistence
        Assert.Single(dequeued);
        Assert.Empty(p.DomainEvents);
    }

    [Fact]
    public void ApplyDiscount_Same_Rate_Does_Not_Raise_Event()
    {
        var p = NewProduct();

        p.ApplyDiscount(DiscountRate.Create(0.10));
        p.DequeueDomainEvents(); // ignore first event

        p.ApplyDiscount(DiscountRate.Create(0.10));

        Assert.Empty(p.DomainEvents);
        Assert.Equal(0.10, p.Discount.Value, 5);
    }

    [Fact]
    public void RemoveDiscount_Clears_Rate_And_Raises_Event_If_Was_Discounted()
    {
        var p = NewProduct();
        p.ApplyDiscount(DiscountRate.Create(0.30));
        p.DequeueDomainEvents(); // ignore applied event

        p.RemoveDiscount();

        Assert.Equal(0.0, p.Discount.Value, 5);
        var ev = Assert.Single(p.DomainEvents);
        Assert.IsType<DiscountRemoved>(ev);
    }

    [Fact]
    public void RemoveDiscount_When_None_Does_Nothing()
    {
        var p = NewProduct();

        p.RemoveDiscount();

        Assert.Equal(0.0, p.Discount.Value, 5);
        Assert.Empty(p.DomainEvents);
    }
}