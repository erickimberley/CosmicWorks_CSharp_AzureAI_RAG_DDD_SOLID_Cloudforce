using CosmicWorks.Domain.ValueObjects;

namespace CosmicWorks.Domain.Events;

/// <summary>
/// Event raised when a product's discount changes from OldRate to NewRate.
/// </summary>
public sealed record DiscountApplied(ProductId ProductId, DiscountRate OldRate, DiscountRate NewRate) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}