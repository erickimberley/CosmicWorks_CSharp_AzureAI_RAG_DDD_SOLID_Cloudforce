using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Events;
using CosmicWorks.Domain.ValueObjects;
using Xunit;

namespace CosmicWorks.Tests.Domain.Entities;

public class ProductIncreaseDiscountTests
{
    private static Product NewProduct() =>
        new(new("P1"), new("C1"), "Bikes", "Road", "RB-1", "fast", new Money(100m));

    [Fact]
    public void Increasing_Discount_Raises_Event_And_Recomputes_SalePrice()
    {
        var p = NewProduct();
        p.ApplyDiscount(DiscountRate.Create(0.10));
        _ = p.DequeueDomainEvents(); // discard first event

        p.ApplyDiscount(DiscountRate.Create(0.30)); // old -> higher new (different branch from decrease)
        var ev = Assert.Single(p.DomainEvents);
        var applied = Assert.IsType<DiscountApplied>(ev);
        Assert.Equal(0.10, applied.OldRate.Value, 6);
        Assert.Equal(0.30, applied.NewRate.Value, 6);
        Assert.Equal(70m, p.SalePrice.Amount);
    }
}