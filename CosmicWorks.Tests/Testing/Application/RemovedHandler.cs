using CosmicWorks.Application.Abstractions;
using CosmicWorks.Domain.Events;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class RemovedHandler : IDomainEventHandler<DiscountRemoved>
{
    public int Count;
    public Task HandleAsync(DiscountRemoved e, CancellationToken ct = default)
    { Count++; return Task.CompletedTask; }
}