using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Events;
using CosmicWorks.Domain.ValueObjects;
using Xunit;

namespace CosmicWorks.Tests.Domain.Entities;

public class ProductRemoveDiscountTwiceTests
{
    private static Product NewProduct() =>
        new(new("P1"), new("C1"), "Bikes", "Road", "RB-1", "fast", new Money(100m));

    [Fact]
    public void Removing_Discount_Twice_Raises_Event_Then_NoOp()
    {
        var p = NewProduct();
        p.ApplyDiscount(DiscountRate.Create(0.20));
        _ = p.DequeueDomainEvents(); // ignore applied

        p.RemoveDiscount();
        var ev = Assert.Single(p.DomainEvents);
        Assert.IsType<DiscountRemoved>(ev);

        _ = p.DequeueDomainEvents();
        p.RemoveDiscount(); // second call is the no-op branch
        Assert.Empty(p.DomainEvents);
    }
}