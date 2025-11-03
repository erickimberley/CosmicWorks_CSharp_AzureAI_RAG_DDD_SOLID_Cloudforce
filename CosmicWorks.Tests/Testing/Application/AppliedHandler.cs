using CosmicWorks.Application.Abstractions;
using CosmicWorks.Domain.Events;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class AppliedHandler : IDomainEventHandler<DiscountApplied>
{
    public int Count;
    public Task HandleAsync(DiscountApplied e, CancellationToken ct = default)
    { Count++; return Task.CompletedTask; }
}