using CosmicWorks.Application.Abstractions;
using CosmicWorks.Domain.Repositories;

namespace CosmicWorks.Application.UseCases;

/// <summary>
/// Use case that removes any discount from products matching the supplied category
/// filter, persists each aggregate, and dispatches resulting domain events.
/// </summary>
public sealed class RemoveProductDiscount
{
    private readonly IProductReader _productReader;
    private readonly IProductWriter _productWriter;
    private readonly IDomainEventDispatcher _events;

    public RemoveProductDiscount(IProductReader productReader, IProductWriter productWriter, IDomainEventDispatcher events)
    {
        _productReader = productReader;
        _productWriter = productWriter;
        _events = events;
    }

    /// <summary>
    /// Removes any discount for all products whose category contains
    /// <paramref name="categoryLike"/> (case-insensitive).
    /// </summary>
    public async Task<int> ExecuteAsync(string categoryLike, CancellationToken ct = default)
    {
        var items = await _productReader.FindByCategoryAsync(categoryLike, ct);
        var updated = 0;

        foreach (var p in items)
        {
            // Skip when already at 0%
            if (p.Discount.Value <= 0.0)
                continue;

            p.RemoveDiscount();

            // Persist first
            await _productWriter.SaveAsync(p, ct);

            // Then publish domain events raised by the aggregate
            var events = p.DequeueDomainEvents();
            if (events.Count > 0)
                await _events.DispatchAsync(events, ct);

            updated++;
        }

        return updated;
    }
}