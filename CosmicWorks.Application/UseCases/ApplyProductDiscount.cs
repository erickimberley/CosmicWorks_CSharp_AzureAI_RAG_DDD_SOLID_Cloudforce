using CosmicWorks.Application.Abstractions;
using CosmicWorks.Domain.Repositories;
using CosmicWorks.Domain.Policies;
using CosmicWorks.Domain.ValueObjects;

namespace CosmicWorks.Application.UseCases;

/// <summary>
/// Use case that applies a requested discount to all products whose category
/// name matches. The requested rate is clamped IDiscountPolicy when necessary. Each updated aggregate is
/// persisted via IProductRepository.SaveAsync and any domain events
/// raised (e.g. DiscountApplied) are dispatched afterwards.
/// </summary>
public sealed class ApplyProductDiscount
{
    private readonly IProductRepository _products;
    private readonly IDomainEventDispatcher _events;
    private readonly IDiscountPolicy _policy;

    public ApplyProductDiscount(
        IProductRepository products,
        IDomainEventDispatcher events,
        IDiscountPolicy policy)
    {
        _products = products;
        _events = events;
        _policy = policy;
    }

    public async Task<ApplyDiscountResult> ExecuteAsync(
        string categoryLike,
        double rate,
        CancellationToken ct = default)
    {
        var requested = DiscountRate.Create(rate);

        var items = await _products.FindByCategoryAsync(categoryLike ?? string.Empty, ct);

        int updated = 0;
        int clamped = 0;

        double? minApplied = null;
        double? maxApplied = null;

        foreach (var p in items)
        {
            var maxAllowed = _policy.MaxFor(p);
            var applied = requested.Value <= maxAllowed.Value ? requested : maxAllowed;

            if (applied.Value < requested.Value)
                clamped++;

            p.ApplyDiscount(applied);

            await _products.SaveAsync(p, ct);

            var evts = p.DequeueDomainEvents();
            if (evts.Count > 0)
                await _events.DispatchAsync(evts, ct);

            minApplied = minApplied is null ? applied.Value : Math.Min(minApplied.Value, applied.Value);
            maxApplied = maxApplied is null ? applied.Value : Math.Max(maxApplied.Value, applied.Value);

            updated++;
        }

        minApplied ??= requested.Value;
        maxApplied ??= requested.Value;

        return new ApplyDiscountResult(
            UpdatedCount: updated,
            ClampedCount: clamped,
            RequestedRate: requested.Value,
            MinAppliedRate: minApplied.Value,
            MaxAppliedRate: maxApplied.Value
        );
    }
}