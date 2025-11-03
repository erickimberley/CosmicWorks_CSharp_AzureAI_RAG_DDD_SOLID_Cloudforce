using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Policies;
using CosmicWorks.Domain.ValueObjects;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class NoopPolicy : IDiscountPolicy 
{ 
    public DiscountRate MaxFor(Product product) => DiscountRate.Create(0.1); 
}