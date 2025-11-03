using CosmicWorks.Application.Abstractions;
using CosmicWorks.Domain.Events;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class SmartProvider : IServiceProvider
{
    private readonly AppliedHandler _a; private readonly RemovedHandler _r;
    public SmartProvider(AppliedHandler a, RemovedHandler r) { _a = a; _r = r; }

    public object? GetService(Type serviceType)
    {
        if (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            var t = serviceType.GetGenericArguments()[0];
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>))
            {
                var arg = t.GetGenericArguments()[0];
                if (arg == typeof(DiscountApplied)) return new object[] { _a };
                if (arg == typeof(DiscountRemoved)) return new object[] { _r };
            }
        }
        return null;
    }
}