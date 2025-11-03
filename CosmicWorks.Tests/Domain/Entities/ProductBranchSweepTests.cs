using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Events;
using CosmicWorks.Domain.ValueObjects;
using Xunit;

namespace CosmicWorks.Tests.Domain.Entities;

public class ProductBranchSweepTests
{
    private static Product NewProduct(decimal price = 100m) =>
        new(new("P1"), new("C1"), "Bikes", "Road", "RB-1", "fast", new Money(price));

    [Fact]
    public void Sweep_Common_Branches_In_Order()
    {
        var p = NewProduct();

        // 1) Remove when not discounted => no-op branch
        p.RemoveDiscount();
        Assert.Empty(p.DomainEvents);

        // 2) Apply small discount => event branch
        p.ApplyDiscount(DiscountRate.Create(0.10));
        Assert.Single(p.DomainEvents);
        p.DequeueDomainEvents();

        // 3) Apply same discount => same-rate no-op branch
        p.ApplyDiscount(DiscountRate.Create(0.10));
        Assert.Empty(p.DomainEvents);

        // 4) Increase discount => different-rate branch (old->new higher)
        p.ApplyDiscount(DiscountRate.Create(0.30));
        var ev = Assert.Single(p.DomainEvents);
        var up = Assert.IsType<DiscountApplied>(ev);
        Assert.Equal(0.10, up.OldRate.Value, 6);
        Assert.Equal(0.30, up.NewRate.Value, 6);
        p.DequeueDomainEvents();

        // 5) Apply zero discount (instead of remove) => different-rate branch to zero
        p.ApplyDiscount(DiscountRate.Create(0.0));
        ev = Assert.Single(p.DomainEvents);
        var down = Assert.IsType<DiscountApplied>(ev);
        Assert.Equal(0.30, down.OldRate.Value, 6);
        Assert.Equal(0.0, down.NewRate.Value, 6);
        p.DequeueDomainEvents();

        // 6) Remove when already zero => no-op branch again
        p.RemoveDiscount();
        Assert.Empty(p.DomainEvents);

        // 7) Re-apply non-zero => event branch
        p.ApplyDiscount(DiscountRate.Create(0.20));
        Assert.Single(p.DomainEvents);
        p.DequeueDomainEvents();

        // 8) Remove when discounted => event branch
        p.RemoveDiscount();
        var rem = Assert.Single(p.DomainEvents);
        Assert.IsType<DiscountRemoved>(rem);
        p.DequeueDomainEvents();

        // 9) Dequeue on empty => empty return branch
        var empty = p.DequeueDomainEvents();
        Assert.Empty(empty);
    }

    [Fact]
    public void EventQueue_Preserves_Order_Across_Multiple_Changes()
    {
        var p = NewProduct();

        p.ApplyDiscount(DiscountRate.Create(0.05));
        p.ApplyDiscount(DiscountRate.Create(0.25));
        p.RemoveDiscount();
        p.ApplyDiscount(DiscountRate.Create(0.15));

        var events = p.DequeueDomainEvents().ToList();
        Assert.Collection(events,
            e => Assert.IsType<DiscountApplied>(e),
            e => Assert.IsType<DiscountApplied>(e),
            e => Assert.IsType<DiscountRemoved>(e),
            e => Assert.IsType<DiscountApplied>(e)
        );
        Assert.Empty(p.DomainEvents);
    }

    [Fact]
    public void Near_One_Discount_On_Tiny_Price_Drives_Alternate_Path()
    {
        var p = NewProduct(price: 0.01m);
        p.ApplyDiscount(DiscountRate.Create(0.999999999)); // different numerical path than typical
        Assert.InRange(p.SalePrice.Amount, 0m, 0.01m);
    }
}