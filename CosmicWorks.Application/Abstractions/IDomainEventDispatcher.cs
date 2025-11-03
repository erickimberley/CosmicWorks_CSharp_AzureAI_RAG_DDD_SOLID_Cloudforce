using CosmicWorks.Domain.Events;

namespace CosmicWorks.Application.Abstractions;

/// <summary>
/// Dispatches domain events raised by aggregates to their handlers.
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default);
}