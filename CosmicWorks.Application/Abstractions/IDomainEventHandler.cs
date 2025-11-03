using CosmicWorks.Domain.Events;

namespace CosmicWorks.Application.Abstractions;

/// <summary>
/// Handles a specific domain event type when dispatched by the application layer.
/// </summary>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent @event, CancellationToken ct = default);
}