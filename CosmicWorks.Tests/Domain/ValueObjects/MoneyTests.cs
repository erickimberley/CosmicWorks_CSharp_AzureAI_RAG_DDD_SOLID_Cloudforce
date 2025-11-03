using CosmicWorks.Domain.ValueObjects;
using Xunit;

namespace CosmicWorks.Tests.Domain.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void ApplyDiscount_Computes_Sale_Price()
    {
        var price = new Money(100m);
        var sale = price.Apply(DiscountRate.Create(0.30));
        Assert.Equal(70m, sale.Amount);
    }

    [Fact]
    public void ApplyDiscount_Zero_Leaves_Amount_Unchanged()
    {
        var price = new Money(42.50m);
        var sale = price.Apply(DiscountRate.Create(0.0));
        Assert.Equal(42.50m, sale.Amount);
    }

    [Fact]
    public void Multiply_Operator_Multiplies_Amount()
    {
        var price = new Money(10m);
        var doubled = price * 2m;
        Assert.Equal(20m, doubled.Amount);
    }

    [Theory]
    [InlineData(12.345, "12.35")]
    [InlineData(12.344, "12.34")]
    [InlineData(0, "0.00")]
    public void ToString_Formats_Two_Decimals(decimal amount, string expected)
    {
        var money = new Money(amount);
        Assert.Equal(expected, money.ToString());
    }
}