using CosmicWorks.Application.UseCases;
using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.ValueObjects;
using CosmicWorks.Tests.Testing.Application;
using FluentAssertions;
using Xunit;

namespace CosmicWorks.Tests.Application.UseCases;

public class ApplyProductDiscountTests
{
    private static Product P(string id = "p1", string catId = "c1", string cat = "Accessories, Helmets", decimal price = 100m)
        => new Product(new ProductId(id), new CategoryId(catId), cat, "Name", "SKU", "desc", new Money(price));

    [Fact]
    public async Task Applies_Requested_When_Within_Cap_And_Dispatches_Event()
    {
        var repo = new FakeRepo(); repo.Items.Add(P());
        var policy = new FakePolicy(0.30);
        var events = new CapturingDispatcher();

        var usecase = new ApplyProductDiscount(repo, events, policy);
        var result = await usecase.ExecuteAsync("helmets", rate: 0.20);

        result.UpdatedCount.Should().Be(1);
        result.ClampedCount.Should().Be(0);
        result.RequestedRate.Should().Be(0.20);
        result.MinAppliedRate.Should().Be(0.20);
        result.MaxAppliedRate.Should().Be(0.20);

        repo.Saved.Should().HaveCount(1);
        repo.Saved[0].Discount.Value.Should().Be(0.20);
        events.Sent.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Clamps_When_Requested_Exceeds_Cap_And_Tracks_Min_Max()
    {
        var repo = new FakeRepo();
        repo.Items.Add(P(id: "p1", price: 200m));
        repo.Items.Add(P(id: "p2", price: 150m));

        var policy = new FakePolicy(0.10);
        var events = new CapturingDispatcher();

        var usecase = new ApplyProductDiscount(repo, events, policy);
        var result = await usecase.ExecuteAsync("helmets", rate: 0.25);

        result.UpdatedCount.Should().Be(2);
        result.ClampedCount.Should().Be(2);
        result.RequestedRate.Should().Be(0.25);
        result.MinAppliedRate.Should().Be(0.10);
        result.MaxAppliedRate.Should().Be(0.10);

        repo.Saved.Select(p => p.Discount.Value).Distinct().Single().Should().Be(0.10);
    }
}