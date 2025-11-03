using CosmicWorks.Application.Abstractions;
using CosmicWorks.Application.UseCases;

namespace CosmicWorks.Tests.Testing.CLI;

internal sealed class FakeCopilot : ICopilotService
{
    public string BasePrompt = "SYS|demo";

    public Task<string> ChatAsync(string prompt, CancellationToken ct = default)
        => Task.FromResult($"echo:{prompt}");

    public Task<IReadOnlyList<string>> ListCategoriesAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<string>>(new List<string> { "Accessories, Helmets", "Bikes, Touring" });

    public Task<ApplyDiscountResult> ApplyDiscountAsync(string categoryLike, double rate, CancellationToken ct = default)
        => Task.FromResult(new ApplyDiscountResult(
            UpdatedCount: 5,
            ClampedCount: rate > 0.30 ? 5 : 0,
            RequestedRate: rate,
            MinAppliedRate: Math.Min(rate, 0.30),
            MaxAppliedRate: Math.Min(rate, 0.30)));

    public Task<int> RemoveDiscountAsync(string categoryLike, CancellationToken ct = default)
        => Task.FromResult(7);

    public string BuildBasePrompt() => BasePrompt;
}