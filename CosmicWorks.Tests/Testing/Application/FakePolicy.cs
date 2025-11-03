using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Policies;
using CosmicWorks.Domain.ValueObjects;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class FakePolicy : IDiscountPolicy
{
    private readonly double _cap;
    public FakePolicy(double cap) => _cap = cap;
    public DiscountRate MaxFor(Product product) => DiscountRate.Create(_cap);
}