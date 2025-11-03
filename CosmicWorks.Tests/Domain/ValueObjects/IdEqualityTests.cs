using CosmicWorks.Domain.ValueObjects;
using Xunit;

namespace CosmicWorks.Tests.Domain.ValueObjects;

public class IdEqualityTests
{
    [Fact]
    public void ProductId_Equality_And_HashCode()
    {
        var a = new ProductId("P-1");
        var b = new ProductId("P-1");
        var c = new ProductId("P-2");

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void CategoryId_Equality_And_HashCode()
    {
        var a = new CategoryId("C-1");
        var b = new CategoryId("C-1");
        var c = new CategoryId("C-2");

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }
}