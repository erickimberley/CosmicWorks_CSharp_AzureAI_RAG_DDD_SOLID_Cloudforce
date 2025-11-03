using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.ValueObjects;

namespace CosmicWorks.Domain.Policies;

/// <summary>
/// Business policy for allowable discounts (category caps).
/// </summary>
public interface IDiscountPolicy
{    
    DiscountRate MaxFor(Product product);
        
    DiscountRate Clamp(Product product, DiscountRate requested) => requested.Value <= MaxFor(product).Value ? requested : MaxFor(product);

    bool IsAllowed(Product product, DiscountRate requested) => requested.Value <= MaxFor(product).Value;
}