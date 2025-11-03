namespace CosmicWorks.Configuration;

/// <summary>
/// Configurable caps for maximum discount rates per category, plus a default
/// cap applied when a category is not explicitly listed.
/// </summary>
public sealed class DiscountPolicySettings
{
    public double DefaultCap { get; init; } = 0.90;

    // Case-insensitive, e.g., { "Accessories, Helmets": 0.30 }
    public Dictionary<string, double> Caps { get; init; } = new(StringComparer.OrdinalIgnoreCase);
}