namespace CosmicWorks.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a product category.
/// </summary>
public readonly record struct CategoryId(string Value)
{
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public override string ToString() => Value;
}