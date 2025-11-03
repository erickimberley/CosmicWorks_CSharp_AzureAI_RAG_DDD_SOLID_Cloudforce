using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.ValueObjects;
using CosmicWorks.Infrastructure.Policies;
using FluentAssertions;
using Xunit;

namespace CosmicWorks.Tests.Infrastructure.Policies;

public class CategoryDiscountPolicyEdgeTests
{
    private static Product Make(string cat, decimal price = 100m) =>
        new Product(new ProductId("p"), new CategoryId("c"), cat, "n", "s", "d", new Money(price));

    [Fact]
    public void Uses_Default_Cap_When_Category_Missing_Or_Whitespace()
    {
        var policy = new CategoryDiscountPolicy(
            caps: new Dictionary<string, double>(),
            defaultCap: 0.15);

        policy.MaxFor(Make("")).Value.Should().Be(0.15);
        policy.MaxFor(Make("   ")).Value.Should().Be(0.15);
        policy.MaxFor(Make(null!)).Value.Should().Be(0.15);
        policy.MaxFor(Make("Other")).Value.Should().Be(0.15);
    }

    [Fact]
    public void Clamps_Default_Cap_And_Rounds_AwayFromZero()
    {
        var policy = new CategoryDiscountPolicy(
            caps: new Dictionary<string, double>(),
            defaultCap: 0.3333333333);

        policy.MaxFor(Make("Bikes")).Value.Should().Be(0.3333);
    }
}