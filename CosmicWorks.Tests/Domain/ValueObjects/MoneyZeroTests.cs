using CosmicWorks.Domain.ValueObjects;
using Xunit;

namespace CosmicWorks.Tests.Domain.ValueObjects;

public class MoneyZeroTests
{
    [Fact]
    public void Zero_Is_Zero()
    {
        Assert.Equal(0m, Money.Zero.Amount);
        Assert.Equal("0.00", Money.Zero.ToString());
    }

    [Fact]
    public void Zero_Stays_Zero_When_Discounted()
    {
        var sale = Money.Zero.Apply(DiscountRate.Create(0.75));
        Assert.Equal(0m, sale.Amount);
    }

    [Fact]
    public void Zero_Stays_Zero_When_Multiplied()
    {
        var times = Money.Zero * 12345m;
        Assert.Equal(0m, times.Amount);
    }
}