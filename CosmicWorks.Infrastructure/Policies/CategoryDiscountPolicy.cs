using CosmicWorks.Configuration;
using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Policies;
using CosmicWorks.Domain.ValueObjects;

namespace CosmicWorks.Infrastructure.Policies;

/// <summary>
/// Creates the policy from configuration.
/// </summary>
internal sealed class CategoryDiscountPolicy : IDiscountPolicy
{
    private readonly IReadOnlyDictionary<string, double> _caps;
    private readonly double _defaultCap;

    internal CategoryDiscountPolicy(DiscountPolicySettings settings)
        : this(settings?.Caps, settings?.DefaultCap ?? 0.90) { }

    // Also allow direct construction for tests
    internal CategoryDiscountPolicy(
        IReadOnlyDictionary<string, double>? caps = null,
        double defaultCap = 0.90)
    {
        _defaultCap = ClampToBusinessRange(defaultCap);

        var dict = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        if (caps is not null)
        {
            foreach (var kv in caps)
            {
                var key = (kv.Key ?? string.Empty).Trim();
                if (key.Length == 0) continue;
                dict[key] = ClampToBusinessRange(kv.Value);
            }
        }
        _caps = dict;
    }

    public DiscountRate MaxFor(Product product)
    {
        if (!string.IsNullOrWhiteSpace(product.CategoryName) &&
            _caps.TryGetValue(product.CategoryName, out var cap))
        {
            return DiscountRate.Create(cap);
        }

        return DiscountRate.Create(_defaultCap);
    }

    // Keep rates sane: 0.00 .. 0.90 (business rule upper bound)
    private static double ClampToBusinessRange(double v)
    {
        if (double.IsNaN(v) || double.IsInfinity(v)) return 0.0;
        if (v < 0.0) return 0.0;
        if (v > 0.90) return 0.90;
        return Math.Round(v, 4, MidpointRounding.AwayFromZero);
    }
}