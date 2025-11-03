namespace CosmicWorks.Domain.ValueObjects;

/// <summary>
/// Fractional discount rate (0 ≤ value < 1). For example, 0.15 = 15% off.
/// </summary>
public readonly record struct DiscountRate
{    
    public static DiscountRate Zero { get; } = new(0.0);

    /// <summary>Create from a decimal fraction (e.g., 0.15 = 15%).</summary>
    public static DiscountRate Create(double value) => new(value);

    public double Value { get; }

    public DiscountRate(double value)
    {
        // 0 ≤ value < 1
        if (value < 0.0 || value >= 1.0)
            throw new ArgumentOutOfRangeException(nameof(value), "Discount must be in [0, 1).");

        Value = value;
    }
}