using System.ComponentModel.DataAnnotations;

namespace CosmicWorks.Configuration;

/// <summary>
/// Strongly-typed settings for Azure Cosmos DB used by the Infrastructure layer
/// to create the CosmosClient, open the database, and resolve the
/// Products container.
/// </summary>
public sealed class CosmosSettings
{
    [Required, Url] public string Endpoint { get; set; } = string.Empty;

    [Required] public string Database { get; set; } = string.Empty;
    
    [Required] public string Container { get; set; } = string.Empty;

    // Require leading slash and at least one char after it, e.g. "/category_id"
    [Required]
    [RegularExpression("^/.+", ErrorMessage = "Cosmos:PartitionKeyPath must start with '/' (e.g., '/category_id').")]
    public string PartitionKeyPath { get; set; } = string.Empty;

    [Required] public string Key { get; set; } = string.Empty;
}