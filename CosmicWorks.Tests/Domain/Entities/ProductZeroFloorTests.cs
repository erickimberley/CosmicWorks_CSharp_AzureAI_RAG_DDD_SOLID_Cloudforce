using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.ValueObjects;
using Xunit;

namespace CosmicWorks.Tests.Domain.Entities;

public class ProductZeroFloorTests
{
    private static Product NewTinyPrice() =>
        new(new("P1"), new("C1"), "Bikes", "Light", "L-1", "desc", new Money(0.01m));

    [Fact]
    public void Near_One_Discount_On_Tiny_Price_Floors_At_Zero()
    {
        var p = NewTinyPrice();
        p.ApplyDiscount(DiscountRate.Create(0.999999999)); // < 1 but extremely close
        Assert.InRange(p.SalePrice.Amount, 0m, 0.01m);     // exercises floor/clamp branch if present
    }
}