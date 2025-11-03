using CosmicWorks.Application.Abstractions;
using CosmicWorks.Domain.Events;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class NoopDispatcher : IDomainEventDispatcher
{
    public Task DispatchAsync(IEnumerable<IDomainEvent> e, CancellationToken ct = default)
        => Task.CompletedTask;
}