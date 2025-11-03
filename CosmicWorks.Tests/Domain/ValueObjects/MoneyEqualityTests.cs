using CosmicWorks.Domain.ValueObjects;
using Xunit;

namespace CosmicWorks.Tests.Domain.ValueObjects;

public class MoneyEqualityTests
{
    [Fact]
    public void Equal_Amounts_Are_Equal_And_HashCodes_Match()
    {
        var a = new Money(12.34m);
        var b = new Money(12.34m);
        Assert.Equal(a, b);
        Assert.True(a.Equals(b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Different_Amounts_Are_Not_Equal()
    {
        var a = new Money(12.34m);
        var b = new Money(12.35m);
        Assert.NotEqual(a, b);
        Assert.False(a.Equals(b));
    }

    [Fact]
    public void Multiply_By_Zero_Yields_Zero()
    {
        var a = new Money(99.99m);
        var z = a * 0m;
        Assert.Equal(0m, z.Amount);
    }

    [Fact]
    public void Very_Large_Amounts_Do_Not_Overflow()
    {
        var large = new Money(9_999_999_999_999.99m);
        var same = large * 1m;
        Assert.Equal(9_999_999_999_999.99m, same.Amount);
    }
}