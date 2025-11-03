using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.ValueObjects;
using CosmicWorks.Infrastructure.Policies;
using FluentAssertions;
using Xunit;

namespace CosmicWorks.Tests.Infrastructure.Policies;

public class CategoryDiscountPolicyMoreTests
{
    private static Product P(string cat) =>
        new Product(new ProductId("p"), new CategoryId("c"), cat, "n", "s", "d", new Money(100m));

    [Fact]
    public void Category_Match_Is_Case_Insensitive()
    {
        var policy = new CategoryDiscountPolicy(
            new Dictionary<string, double> { ["Accessories, Helmets"] = 0.30 },
            defaultCap: 0.10);

        policy.MaxFor(P("accessories, helmets")).Value.Should().Be(0.30);
        policy.MaxFor(P("ACCESSORIES, HELMETS")).Value.Should().Be(0.30);
    }

    [Theory]
    [InlineData(double.NaN, 0.00)]
    [InlineData(double.NegativeInfinity, 0.00)]
    [InlineData(-1.0, 0.00)]
    [InlineData(0.95, 0.90)] // clamped to business upper bound
    public void Default_Cap_Clamp_Edge_Cases(double configured, double expected)
    {
        var policy = new CategoryDiscountPolicy(new Dictionary<string, double>(), configured);
        policy.MaxFor(P("Other")).Value.Should().Be(expected);
    }
}