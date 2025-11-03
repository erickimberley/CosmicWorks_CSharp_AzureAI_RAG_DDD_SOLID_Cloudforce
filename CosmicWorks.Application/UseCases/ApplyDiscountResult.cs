namespace CosmicWorks.Application.UseCases;

/// <summary>
/// Outcome of an apply discount operation.
/// For mixed-category selections, Min/Max capture the applied range.
/// </summary>
public sealed record ApplyDiscountResult(
    int UpdatedCount,
    int ClampedCount,
    double RequestedRate,
    double MinAppliedRate,
    double MaxAppliedRate);