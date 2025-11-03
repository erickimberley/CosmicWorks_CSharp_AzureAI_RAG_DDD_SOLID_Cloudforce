using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Events;
using CosmicWorks.Domain.ValueObjects;
using Xunit;

namespace CosmicWorks.Tests.Domain.Entities;

public class ProductEdgeCasesTests
{
    private static Product NewProduct(
        string id = "P1", string catId = "C1", string categoryName = "Bikes",
        string name = "Road Bike", string sku = "RB-1", string desc = "fast", decimal price = 100m)
        => new Product(new ProductId(id), new CategoryId(catId), categoryName, name, sku, desc, new Money(price));

    [Fact]
    public void ApplyDiscount_Twice_With_Different_Rate_Raises_Event_With_Old_And_New()
    {
        var p = NewProduct(price: 100m);
        p.ApplyDiscount(DiscountRate.Create(0.30));
        _ = p.DequeueDomainEvents(); // ignore first event

        p.ApplyDiscount(DiscountRate.Create(0.10));

        var ev = Assert.Single(p.DomainEvents);
        var applied = Assert.IsType<DiscountApplied>(ev);
        Assert.Equal(0.30, applied.OldRate.Value, 6);
        Assert.Equal(0.10, applied.NewRate.Value, 6);
        Assert.Equal(90m, p.SalePrice.Amount);
    }

    [Fact]
    public void ApplyDiscount_Very_Close_To_One_Leaves_NonNegative_SalePrice()
    {
        var p = NewProduct(price: 1m);
        p.ApplyDiscount(DiscountRate.Create(0.999999999)); // allowed: < 1.0

        Assert.InRange(p.SalePrice.Amount, 0m, 1m);
    }
}