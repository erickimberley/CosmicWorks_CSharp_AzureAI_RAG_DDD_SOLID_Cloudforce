namespace CosmicWorks.Domain.ValueObjects;

/// <summary>
/// Strongly-typed product identifier.
/// </summary>
public readonly record struct ProductId(string Value)
{
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public override string ToString() => Value;
}