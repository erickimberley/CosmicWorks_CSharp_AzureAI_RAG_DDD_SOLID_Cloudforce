using CosmicWorks.Domain.ValueObjects;
using Xunit;

namespace CosmicWorks.Tests.Domain.ValueObjects;

public class IdValueObjectTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ProductId_IsEmpty_For_NullOrWhitespace(string? raw)
    {
        var id = new ProductId(raw!);
        Assert.True(id.IsEmpty);
    }

    [Fact]
    public void ProductId_Not_Empty_When_Has_Text()
    {
        var id = new ProductId("ABC");
        Assert.False(id.IsEmpty);
        Assert.Equal("ABC", id.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void CategoryId_IsEmpty_For_NullOrWhitespace(string? raw)
    {
        var cid = new CategoryId(raw!);
        Assert.True(cid.IsEmpty);
    }

    [Fact]
    public void CategoryId_Not_Empty_When_Has_Text()
    {
        var cid = new CategoryId("C1");
        Assert.False(cid.IsEmpty);
        Assert.Equal("C1", cid.ToString());
    }
}