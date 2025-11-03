using CosmicWorks.Application.UseCases;

namespace CosmicWorks.Application.Abstractions;

/// <summary>
/// Command-style application operations related to discounts.
/// </summary>
public interface IDiscountCommands
{
    Task<ApplyDiscountResult> ApplyDiscountAsync(string categoryLike, double rate, CancellationToken ct = default);

    Task<int> RemoveDiscountAsync(string categoryLike, CancellationToken ct = default);
}