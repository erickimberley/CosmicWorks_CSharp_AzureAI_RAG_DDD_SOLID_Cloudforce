using Newtonsoft.Json;

namespace CosmicWorks.Infrastructure.Integrations.Persistance.Cosmos;

/// <summary>
/// Storage model for the Cosmos DB Products container.
/// Property names map 1:1 to persisted JSON fields.
/// </summary>
internal sealed class CosmosProductDocument
{
    [JsonProperty("id")]
    public string Id { get; set; } = default!;
        
    [JsonProperty("category_id")]
    public string CategoryId { get; set; } = default!;
                
    [JsonProperty("category_name")]
    public string? CategoryName { get; set; }
                
    [JsonProperty("sku")]
    public string? Sku { get; set; }
                
    [JsonProperty("name")]
    public string? Name { get; set; }
                
    [JsonProperty("description")]
    public string? Description { get; set; }
                
    [JsonProperty("price")]
    public double Price { get; set; }
                
    [JsonProperty("discount")]
    public double Discount { get; set; }
        
    [JsonProperty("sale_price")]
    public double? SalePrice { get; set; }

    [JsonProperty("_etag")]
    public string? ETag { get; set; }
}