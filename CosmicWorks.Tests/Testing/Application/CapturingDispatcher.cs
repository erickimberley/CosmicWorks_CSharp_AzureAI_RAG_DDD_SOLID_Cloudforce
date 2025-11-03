using CosmicWorks.Application.Abstractions;
using CosmicWorks.Domain.Events;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class CapturingDispatcher : IDomainEventDispatcher
{
    public readonly List<IDomainEvent> Sent = new();
    public Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default)
    {
        Sent.AddRange(events);
        return Task.CompletedTask;
    }
}