using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Events;
using CosmicWorks.Domain.ValueObjects;
using Xunit;

namespace CosmicWorks.Tests.Domain.Entities;

public class ProductApplyZeroAfterDiscountTests
{
    private static Product NewProduct() =>
        new Product(
            new ProductId("P1"),
            new CategoryId("C1"),
            "Bikes",
            "Road Bike",
            "RB-1",
            "fast",
            new Money(100m));

    [Fact]
    public void ApplyDiscount_Zero_After_Discounted_Raises_Event_And_Resets_SalePrice()
    {
        var p = NewProduct();

        // Start with a discount
        p.ApplyDiscount(DiscountRate.Create(0.20));
        _ = p.DequeueDomainEvents(); // ignore first event

        // Now apply 0.0 (instead of using RemoveDiscount)
        p.ApplyDiscount(DiscountRate.Create(0.0));

        // Event reflects change from 0.20 -> 0.00
        var ev = Assert.Single(p.DomainEvents);
        var applied = Assert.IsType<DiscountApplied>(ev);
        Assert.Equal(0.20, applied.OldRate.Value, 6);
        Assert.Equal(0.0, applied.NewRate.Value, 6);

        // Sale price resets to full price
        Assert.Equal(100m, p.SalePrice.Amount);
    }
}