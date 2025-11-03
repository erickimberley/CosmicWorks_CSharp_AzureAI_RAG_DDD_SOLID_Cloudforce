using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Policies;
using CosmicWorks.Domain.ValueObjects;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class Policy : IDiscountPolicy
{
    private readonly double _cap;
    public Policy(double cap) => _cap = cap;
    public DiscountRate MaxFor(Product p) => DiscountRate.Create(_cap);
}