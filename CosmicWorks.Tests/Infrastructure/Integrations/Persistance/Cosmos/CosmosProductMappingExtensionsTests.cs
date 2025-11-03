using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.ValueObjects;
using CosmicWorks.Infrastructure.Integrations.Persistance.Cosmos;
using FluentAssertions;
using Xunit;

namespace CosmicWorks.Tests.Infrastructure.Integrations.Persistance.Cosmos;

public class CosmosProductMappingExtensionsTests
{
    [Fact]
    public void Document_ToDomain_Handles_Nulls_And_Maps_Primitives()
    {
        var doc = new CosmosProductDocument
        {
            Id = "P-1",
            CategoryId = "C-1",
            CategoryName = null,
            Sku = null,
            Name = null,
            Description = null,
            Price = 742.35,
            Discount = 0.0,
            SalePrice = null,
            ETag = "etag-123"
        };

        var p = doc.ToDomain();

        p.Id.Value.Should().Be("P-1");
        p.CategoryId.Value.Should().Be("C-1");
        p.CategoryName.Should().NotBeNull();
        p.Sku.Should().NotBeNull();
        p.Name.Should().NotBeNull();
        p.Description.Should().NotBeNull();

        p.Price.Amount.Should().Be(742.35m);
        p.Discount.Value.Should().Be(0.0);
        // Domain keeps SalePrice == Price when discount == 0
        p.SalePrice.Amount.Should().Be(p.Price.Amount);
    }

    [Fact]
    public void Row_ToDomain_Maps_And_Applies_Discount()
    {
        var row = new ProductSimilarityRow
        {
            Id = "P-2",
            CategoryId = "C-1",
            CategoryName = "Bikes",
            Sku = "BK-0002",
            Name = "Touring-1000",
            Description = "blue",
            Price = 100.00,
            Discount = 0.25,     
            SalePrice = 75.0
        };

        var p = row.ToDomain();

        p.Id.Value.Should().Be("P-2");
        p.CategoryName.Should().Be("Bikes");
        p.Price.Amount.Should().Be(100.00m);
        p.Discount.Value.Should().Be(0.25);
        p.SalePrice.Amount.Should().Be(75.00m);
    }

    [Fact]
    public void Domain_ToDocument_Maps_All_Fields_And_Computes_SalePrice()
    {
        var p = new Product(new ProductId("P-3"), new CategoryId("C-9"), "Accessories, Helmets",
                            "Helmet", "HL-1", "safe!", new Money(50m));

        p.ApplyDiscount(DiscountRate.Create(0.10)); 

        var doc = p.ToDocument(etag: "tag");

        doc.Id.Should().Be("P-3");
        doc.CategoryId.Should().Be("C-9");
        doc.CategoryName.Should().Be("Accessories, Helmets");
        doc.Sku.Should().Be("HL-1");
        doc.Name.Should().Be("Helmet");
        doc.Description.Should().Be("safe!");
        doc.Price.Should().Be(50.0);
        doc.Discount.Should().Be(0.10);
        // Price * (1 - discount)
        doc.SalePrice.Should().Be(45.0);
        doc.ETag.Should().Be("tag");
    }

    [Fact]
    public void ToDocument_FromDomain_Maps_Everything_Including_SalePrice_And_Etag()
    {
        var p = new Product(
            new ProductId("P-002"),
            new CategoryId("C-TOUR"),
            "Bikes, Touring",
            "Touring-3000",
            "BK-T18U-50",
            "Discover the excitement",
            new Money(100m));

        // 20% off → sale price 80
        p.ApplyDiscount(DiscountRate.Create(0.20));

        var d = p.ToDocument(etag: "etag-999");

        d.Id.Should().Be("P-002");
        d.CategoryId.Should().Be("C-TOUR");
        d.CategoryName.Should().Be("Bikes, Touring");
        d.Sku.Should().Be("BK-T18U-50");
        d.Name.Should().Be("Touring-3000");
        d.Description.Should().Be("Discover the excitement");
        d.Price.Should().Be(100.0);
        d.Discount.Should().BeApproximately(0.20, 1e-9);
        d.SalePrice.Should().BeApproximately(80.0, 1e-9);
        d.ETag.Should().Be("etag-999");
    }    
}