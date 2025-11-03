using CosmicWorks.Application.Events;
using CosmicWorks.Domain.Events;
using CosmicWorks.Domain.ValueObjects;
using CosmicWorks.Tests.Testing.Application;
using FluentAssertions;
using Xunit;

namespace CosmicWorks.Tests.Application.Events;

public class DomainEventDispatcherMoreTests
{    
    [Fact]
    public async Task Dispatches_By_Exact_Generic_Type_For_Each_Event_In_Batch()
    {
        var a = new AppliedHandler();
        var r = new RemovedHandler();
        var provider = new SmartProvider(a, r);
        var dispatcher = new DomainEventDispatcher(provider);

        var evts = new IDomainEvent[]
        {
            new DiscountApplied(new ProductId("p1"), DiscountRate.Zero, DiscountRate.Create(0.1)),
            new DiscountRemoved(new ProductId("p2"), DiscountRate.Create(0.2))
        };

        await dispatcher.DispatchAsync(evts);

        a.Count.Should().Be(1);
        r.Count.Should().Be(1);
    }
}