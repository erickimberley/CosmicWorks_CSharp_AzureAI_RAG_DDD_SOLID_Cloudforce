using CosmicWorks.Configuration;
using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.ValueObjects;
using CosmicWorks.Infrastructure.Policies;
using FluentAssertions;
using Xunit;

namespace CosmicWorks.Tests.Policies;

public class CategoryDiscountPolicyTests
{
    private static Product Make(string cat) =>
        new Product(new ProductId("p"), new CategoryId("c"), cat, "n", "s", "d", new Money(10));

    [Fact]
    public void Uses_Configured_Cap_For_Matching_Category_CaseInsensitive()
    {
        var caps = new Dictionary<string, double>
        {
            ["Accessories, Helmets"] = 0.30
        };

        var policy = new CategoryDiscountPolicy(caps, defaultCap: 0.9);
        var max = policy.MaxFor(Make("accessories, helmets"));

        max.Value.Should().BeApproximately(0.30, 1e-9);
    }

    [Fact]
    public void Falls_Back_To_Default_When_Category_Not_Configured()
    {
        var policy = new CategoryDiscountPolicy(caps: null, defaultCap: 0.15);
        var max = policy.MaxFor(Make("Unknown"));
        max.Value.Should().BeApproximately(0.15, 1e-9);
    }

    [Fact]
    public void Clamps_And_Rounds_Caps_And_Defaults()
    {
        var caps = new Dictionary<string, double>
        {
            ["Bad1"] = -1.0,
            ["Bad2"] = double.NaN,
            ["Bad3"] = double.PositiveInfinity,
            ["TooBig"] = 1.234567 // should clamp to 0.90 and round
        };

        var policy = new CategoryDiscountPolicy(caps, defaultCap: 1.5);

        policy.MaxFor(Make("Bad1")).Value.Should().Be(0.0);
        policy.MaxFor(Make("Bad2")).Value.Should().Be(0.0);
        policy.MaxFor(Make("Bad3")).Value.Should().Be(0.0);
        policy.MaxFor(Make("TooBig")).Value.Should().Be(0.90);
        policy.MaxFor(Make("Other")).Value.Should().Be(0.90);
    }

    private static Product ProductWithCat(string cat) =>
        new Product(new ProductId("P1"), new CategoryId("C1"), cat, "Name", "SKU", "Desc", new Money(10m));

    [Fact]
    public void Uses_Category_Cap_And_Default_Cap()
    {
        var settings = new DiscountPolicySettings
        {
            DefaultCap = 0.80,
            Caps = new Dictionary<string, double> { ["Accessories, Helmets"] = 0.30 }
        };
        var policy = new CategoryDiscountPolicy(settings);

        policy.MaxFor(ProductWithCat("Accessories, Helmets")).Value.Should().Be(0.30);
        policy.MaxFor(ProductWithCat("Unknown Category")).Value.Should().Be(0.80);
    }

    [Fact]
    public void Clamps_And_Rounds_Weird_Values()
    {
        var policy = new CategoryDiscountPolicy(
            new Dictionary<string, double>
            {
                ["TooHigh"] = 1.2,
                ["Negative"] = -0.7,
                ["Weird"] = double.NaN
            },
            defaultCap: 0.93333 // will clamp to 0.90
        );

        policy.MaxFor(ProductWithCat("TooHigh")).Value.Should().Be(0.90);
        policy.MaxFor(ProductWithCat("Negative")).Value.Should().Be(0.00);
        policy.MaxFor(ProductWithCat("Weird")).Value.Should().Be(0.00);
        policy.MaxFor(ProductWithCat("NoMatch")).Value.Should().Be(0.90); // default clamped
    }
}