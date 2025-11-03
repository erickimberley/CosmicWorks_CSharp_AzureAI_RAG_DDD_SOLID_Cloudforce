using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Events;
using CosmicWorks.Domain.ValueObjects;
using Xunit;

namespace CosmicWorks.Tests.Domain.Entities;

public class ProductEventOrderingTests
{
    private static Product NewProduct() =>
        new(new("P1"), new("C1"), "Bikes", "Road", "RB-1", "fast", new Money(100m));

    [Fact]
    public void Multiple_Events_Are_Queued_Then_Dequeued_In_Order()
    {
        var p = NewProduct();

        p.ApplyDiscount(DiscountRate.Create(0.10));
        p.ApplyDiscount(DiscountRate.Create(0.20));
        p.RemoveDiscount();

        var events = p.DequeueDomainEvents().ToList();
        Assert.Collection(events,
            e => Assert.IsType<DiscountApplied>(e),
            e => Assert.IsType<DiscountApplied>(e),
            e => Assert.IsType<DiscountRemoved>(e)
        );

        // Queue is cleared after dequeue (branch in dequeue method)
        Assert.Empty(p.DomainEvents);
    }
}