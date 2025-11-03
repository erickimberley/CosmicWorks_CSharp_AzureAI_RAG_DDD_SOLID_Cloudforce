namespace CosmicWorks.Domain.ValueObjects;

/// <summary>
/// Immutable value object representing a monetary amount in the system's base currency.
/// </summary>
public readonly record struct Money(decimal Amount)
{
    public static Money Zero => new(0m);

    public static Money operator *(Money m, decimal factor) => new(m.Amount * factor);

    public Money Apply(DiscountRate rate) => new(Amount * (1 - (decimal)rate.Value));

    public override string ToString() => Amount.ToString("0.00");
}