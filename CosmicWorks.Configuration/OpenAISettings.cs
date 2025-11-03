namespace CosmicWorks.Configuration;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Azure OpenAI configuration used for chat completions and embeddings.
/// </summary>
public sealed class OpenAISettings
{
    [Required, Url]
    public string Endpoint { get; set; } = string.Empty;

    [Required]
    public string ApiVersion { get; set; } = string.Empty;

    [Required]
    public string ChatDeployment { get; set; } = string.Empty;

    [Required]
    public string EmbeddingDeployment { get; set; } = string.Empty;

    [Required]
    public string ApiKey { get; set; } = string.Empty;
}