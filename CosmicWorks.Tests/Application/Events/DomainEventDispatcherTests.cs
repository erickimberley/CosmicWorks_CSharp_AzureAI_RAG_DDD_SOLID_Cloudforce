using CosmicWorks.Application.Events;
using CosmicWorks.Domain.Events;
using CosmicWorks.Domain.ValueObjects;
using CosmicWorks.Tests.Testing.Application;
using FluentAssertions;
using Xunit;

namespace CosmicWorks.Tests.Application.Events;

public class DomainEventDispatcherTests
{
    [Fact]
    public async Task Dispatches_To_All_Registered_Handlers_For_Type()
    {
        var h1 = new Handler(); var h2 = new Handler();
        var provider = new Provider(new[] { h1, h2 });
        var dispatcher = new DomainEventDispatcher(provider);

        var ev = new DiscountApplied(new ProductId("p"), DiscountRate.Zero, DiscountRate.Create(0.1));
        await dispatcher.DispatchAsync(new[] { ev });

        h1.Count.Should().Be(1);
        h2.Count.Should().Be(1);
    }

    [Fact]
    public async Task No_Handlers_Does_Not_Throw()
    {
        var provider = new Provider(null!);
        var dispatcher = new DomainEventDispatcher(provider);

        await dispatcher.DispatchAsync(Array.Empty<IDomainEvent>());
        await dispatcher.DispatchAsync(new IDomainEvent[] { new DiscountRemoved(new ProductId("p"), DiscountRate.Zero) });
    }
}