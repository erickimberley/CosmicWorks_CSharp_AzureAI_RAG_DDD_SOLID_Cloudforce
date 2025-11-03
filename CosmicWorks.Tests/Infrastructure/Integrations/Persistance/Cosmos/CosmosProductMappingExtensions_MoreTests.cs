using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.ValueObjects;
using CosmicWorks.Infrastructure.Integrations.Persistance.Cosmos;
using FluentAssertions;
using Xunit;

namespace CosmicWorks.Tests.Infrastructure.Integrations.Persistance.Cosmos;

public class CosmosProductMappingExtensions_MoreTests
{
    [Fact]
    public void ToDocument_Without_Discount_Still_Maps_SalePrice_As_Price()
    {
        var p = new Product(
            new ProductId("P-10"),
            new CategoryId("C-10"),
            "Cat",
            "Name",
            "SKU",
            "Desc",
            new Money(50m));

        // no discount -> sale price = price
        var d = p.ToDocument(etag: "e1");

        d.Price.Should().Be(50.0);
        d.Discount.Should().Be(0.0);
        d.SalePrice.Should().Be(50.0);
        d.ETag.Should().Be("e1");
    }
}