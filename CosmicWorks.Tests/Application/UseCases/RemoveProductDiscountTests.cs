using CosmicWorks.Application.UseCases;
using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.ValueObjects;
using CosmicWorks.Tests.Testing.Application;
using FluentAssertions;
using Xunit;

namespace CosmicWorks.Tests.Application.UseCases;

public class RemoveProductDiscountTests
{    
    private static Product P(double discount, string id = "p")
    {
        var p = new Product(new ProductId(id), new CategoryId("c"), "Accessories, Helmets", "n", "s", "d", new Money(100m));
        if (discount > 0) p.ApplyDiscount(DiscountRate.Create(discount));
        return p;
    }

    [Fact]
    public async Task Removes_Discount_Only_When_Greater_Than_Zero()
    {
        var rw = new FakeReaderWriter();
        rw.Items.Add(P(0.15, "p1")); // will be removed
        rw.Items.Add(P(0.00, "p2")); // skipped

        var events = new CapturingDispatcher();
        var usecase = new RemoveProductDiscount(rw, rw, events);

        var updated = await usecase.ExecuteAsync("helmets");

        updated.Should().Be(1);
        rw.Saved.Should().HaveCount(1);
        rw.Saved[0].Discount.Value.Should().Be(0.0);
        events.Sent.Should().NotBeEmpty(); // DiscountRemoved raised
    }
}