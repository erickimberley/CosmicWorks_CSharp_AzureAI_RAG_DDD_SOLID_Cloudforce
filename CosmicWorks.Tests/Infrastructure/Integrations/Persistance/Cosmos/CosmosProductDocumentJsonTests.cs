using CosmicWorks.Infrastructure.Integrations.Persistance.Cosmos;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace CosmicWorks.Tests.Infrastructure.Integrations.Persistance.Cosmos;

public class CosmosProductDocumentJsonTests
{
    [Fact]
    public void Serializes_With_Expected_Json_Property_Names()
    {
        var doc = new CosmosProductDocument
        {
            Id = "DDD64AA0-...",
            CategoryId = "75BF1ACB-...",
            CategoryName = "Bikes, Touring Bikes",
            Sku = "BK-T18U-50",
            Name = "Touring-3000 Blue, 50\"",
            Description = "Discover ...",
            Price = 742.35,
            Discount = 0.0,
            SalePrice = null,
            ETag = "\"00000000-0000-0000-ABCD-000000000000\""
        };

        var json = JsonConvert.SerializeObject(doc);

        json.Should().Contain("\"id\"");
        json.Should().Contain("\"category_id\"");
        json.Should().Contain("\"category_name\"");
        json.Should().Contain("\"sku\"");
        json.Should().Contain("\"name\"");
        json.Should().Contain("\"description\"");
        json.Should().Contain("\"price\"");
        json.Should().Contain("\"discount\"");
        json.Should().Contain("\"sale_price\"");
        json.Should().Contain("\"_etag\"");
        json.Should().Contain("\"sale_price\":null"); // when Discount == 0
    }    
}