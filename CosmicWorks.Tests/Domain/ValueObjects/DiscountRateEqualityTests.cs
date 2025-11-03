using CosmicWorks.Domain.ValueObjects;
using Xunit;

namespace CosmicWorks.Tests.Domain.ValueObjects;

public class DiscountRateEqualityTests
{
    [Fact]
    public void Equal_Rates_Are_Equal_And_HashCodes_Match()
    {
        var a = DiscountRate.Create(0.20);
        var b = DiscountRate.Create(0.20);
        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Different_Rates_Are_Not_Equal()
    {
        var a = DiscountRate.Create(0.20);
        var b = DiscountRate.Create(0.21);
        Assert.NotEqual(a, b);
    }
}