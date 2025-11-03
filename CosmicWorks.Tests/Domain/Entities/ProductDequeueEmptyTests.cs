using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.ValueObjects;
using Xunit;

namespace CosmicWorks.Tests.Domain.Entities;

public class ProductDequeueEmptyTests
{
    private static Product NewProduct() =>
        new Product(
            new ProductId("P1"),
            new CategoryId("C1"),
            "Bikes",
            "Road Bike",
            "RB-1",
            "fast",
            new Money(100m));

    [Fact]
    public void DequeueDomainEvents_When_Empty_Returns_Empty_List()
    {
        var p = NewProduct();

        var dequeued = p.DequeueDomainEvents();

        Assert.NotNull(dequeued);
        Assert.Empty(dequeued);
        Assert.Empty(p.DomainEvents);
    }
}