namespace CosmicWorks.Domain.Events;

/// <summary>
/// Abstraction for domain events raised by entities/aggregates.
/// </summary>
public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}