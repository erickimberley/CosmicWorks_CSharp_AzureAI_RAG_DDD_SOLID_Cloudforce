using CosmicWorks.Domain.ValueObjects;
using Xunit;

namespace CosmicWorks.Tests.Domain.ValueObjects;

public class DiscountRateTests
{
    [Fact]
    public void Create_Allows_Zero_And_Fraction_Less_Than_One()
    {
        var zero = DiscountRate.Create(0.0);
        Assert.Equal(0.0, zero.Value);

        var quarter = DiscountRate.Create(0.25);
        Assert.Equal(0.25, quarter.Value, 10);
    }

    [Theory]
    [InlineData(-0.0000001)]
    [InlineData(-0.01)]
    [InlineData(1.0)]
    [InlineData(1.0000001)]
    public void Create_Throws_For_OutOfRange(double value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => DiscountRate.Create(value));
    }

    [Fact]
    public void Create_Allows_Just_Below_One()
    {
        var near = DiscountRate.Create(0.999999999);
        Assert.True(near.Value < 1.0);
        Assert.Equal(0.999999999, near.Value, 12);
    }
}