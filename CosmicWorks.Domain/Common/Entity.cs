using CosmicWorks.Domain.Events;

namespace CosmicWorks.Domain.Common;

/// <summary>
/// Base class for aggregate roots and entities that raise domain events.
/// </summary>
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

    protected void Raise(IDomainEvent @event) => _domainEvents.Add(@event);

    /// <summary>Returns and clears queued events (for dispatch after persistence).</summary>
    public IReadOnlyList<IDomainEvent> DequeueDomainEvents()
    {
        var copy = _domainEvents.ToArray();
        _domainEvents.Clear();
        return copy;
    }
}