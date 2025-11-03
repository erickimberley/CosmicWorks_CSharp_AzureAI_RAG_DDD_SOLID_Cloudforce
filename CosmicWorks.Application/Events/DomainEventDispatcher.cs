using CosmicWorks.Application.Abstractions;
using CosmicWorks.Domain.Events;

namespace CosmicWorks.Application.Events;

/// <summary>
/// Reflection-based dispatcher that resolves all IDomainEventHandler{TEvent}
/// instances for each event and invokes them sequentially.
/// </summary>
public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _provider;

    public DomainEventDispatcher(IServiceProvider provider) => _provider = provider;

    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default)
    {
        foreach (var e in events)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(e.GetType());
            var enumerableHandlerType = typeof(IEnumerable<>).MakeGenericType(handlerType);

            var handlers = (IEnumerable<object>?)_provider.GetService(enumerableHandlerType)
                          ?? Enumerable.Empty<object>();

            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync))!;
                var task = (Task)method.Invoke(handler, new object[] { e, ct })!;
                await task.ConfigureAwait(false);
            }
        }
    }
}