using CosmicWorks.Application.Abstractions;
using CosmicWorks.Application.UseCases;

namespace CosmicWorks.Tests.Testing.CLI;

/// <summary>
/// Spy/fake for the CLI-facing copilot facade. Records calls and parameters.
/// </summary>
internal sealed class RecordingCopilot : ICopilotService
{
    public string? LastCategory { get; private set; }
    public double? LastRate { get; private set; }

    public bool Applied { get; private set; }
    public bool Removed { get; private set; }
    public bool Listed { get; private set; }
    public bool Chatted { get; private set; }
    public bool AskedPrompt { get; private set; }

    public Task<ApplyDiscountResult> ApplyDiscountAsync(string categoryLike, double rate, CancellationToken ct = default)
    {
        Applied = true;
        LastCategory = categoryLike;
        LastRate = rate;

        // Return a benign result; tests assert on parameters, not copy text.
        return Task.FromResult(new ApplyDiscountResult(
            UpdatedCount: 0,
            ClampedCount: 0,
            RequestedRate: rate,
            MinAppliedRate: rate,
            MaxAppliedRate: rate
        ));
    }

    public Task<int> RemoveDiscountAsync(string categoryLike, CancellationToken ct = default)
    {
        Removed = true;
        LastCategory = categoryLike;
        return Task.FromResult(0);
    }

    public Task<IReadOnlyList<string>> ListCategoriesAsync(CancellationToken ct = default)
    {
        Listed = true;
        return Task.FromResult<IReadOnlyList<string>>(new List<string> { "A", "B" });
    }

    public Task<string> ChatAsync(string prompt, CancellationToken ct = default)
    {
        Chatted = true;
        return Task.FromResult("ok");
    }

    public string BuildBasePrompt()
    {
        AskedPrompt = true;
        return "SYSTEM";
    }
}