using System.ComponentModel.DataAnnotations;

namespace CosmicWorks.Configuration;

/// <summary>
/// Retrieval-augmented generation (RAG) tuning parameters for the catalog chat.
/// </summary>
public sealed class RagSettings
{
    [Range(1, 10)]
    public int TopK { get; set; } = 3;

    [Range(0.0, 1.0)]
    public double MinSimilarity { get; set; } = 0.25;
}