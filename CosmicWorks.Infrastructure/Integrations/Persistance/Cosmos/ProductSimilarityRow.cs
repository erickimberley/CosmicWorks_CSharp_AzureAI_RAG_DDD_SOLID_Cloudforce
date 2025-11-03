using Newtonsoft.Json;

namespace CosmicWorks.Infrastructure.Integrations.Persistance.Cosmos;


/// <summary>
/// DTO for a single row returned by the Cosmos vector search query,
/// including the materialized product fields and the similarity_score.
/// </summary>
internal sealed class ProductSimilarityRow
{
    [JsonProperty("id")]
    public string Id { get; set; } = default!;

    [JsonProperty("category_id")]
    public string CategoryId { get; set; } = default!;

    [JsonProperty("category_name")]
    public string CategoryName { get; set; } = default!;

    [JsonProperty("sku")]
    public string Sku { get; set; } = default!;

    [JsonProperty("name")]
    public string Name { get; set; } = default!;

    [JsonProperty("description")]
    public string Description { get; set; } = default!;

    [JsonProperty("price")]
    public double Price { get; set; }

    [JsonProperty("discount")]
    public double Discount { get; set; }

    [JsonProperty("sale_price")]
    public double? SalePrice { get; set; }
        
    [JsonProperty("similarity_score")]
    public double SimilarityScore { get; set; }
        
    [JsonProperty("_etag")]
    public string? ETag { get; set; }
}