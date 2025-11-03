using CosmicWorks.Domain.ValueObjects;

namespace CosmicWorks.Domain.Events;

/// <summary>
/// Event raised when a product's discount is removed.
/// </summary>
public sealed record DiscountRemoved(ProductId ProductId, DiscountRate OldRate) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}