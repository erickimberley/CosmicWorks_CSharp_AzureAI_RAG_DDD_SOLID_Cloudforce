using CosmicWorks.Application.Abstractions;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CosmicWorks.CLI;

/// <summary>
/// Minimal, dependency-free parser -> calls ICopilotService.
/// Keep it dumb and testable; all business logic stays in use-cases.
/// </summary>
public sealed class CommandRouter : ICommandRouter
{
    private static readonly Regex DiscountCmd =
        new(@"^discount\s+(.+?)\s+([0-9]+(?:\.[0-9]+)?%?)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Regex RemoveCmd =
        new(@"^remove\s+(.+?)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Regex ListCategoriesCmd =
        new(@"^(list\s+categories|categories)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Regex PromptCmd =
        new(@"^prompt\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private readonly ICopilotService _copilot;

    public CommandRouter(ICopilotService copilot) => _copilot = copilot;

    public async Task<string?> RouteAsync(string input, CancellationToken ct = default)
    {
        var line = (input ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(line)) return string.Empty;

        // list categories
        if (ListCategoriesCmd.IsMatch(line))
        {
            var cats = await _copilot.ListCategoriesAsync(ct);
            return cats.Count == 0
                ? "No categories found."
                : "- " + string.Join(Environment.NewLine + "- ", cats);
        }

        // prompt (show current system prompt template)
        if (PromptCmd.IsMatch(line))
        {
            return _copilot.BuildBasePrompt();
        }

        // discount "<categoryLike>" <rate or percent>
        var mDiscount = DiscountCmd.Match(line);
        if (mDiscount.Success)
        {
            var categoryLike = mDiscount.Groups[1].Value.Trim();
            var rateText = mDiscount.Groups[2].Value.Trim();

            var requestedRate = ParseRate(rateText);

            var result = await _copilot.ApplyDiscountAsync(categoryLike, requestedRate, ct);

            // format pieces
            const double eps = 1e-9;
            var requested = ToPercent(result.RequestedRate);
            var minApplied = ToPercent(result.MinAppliedRate);
            var maxApplied = ToPercent(result.MaxAppliedRate);

            var adjusted =
                Math.Abs(result.MinAppliedRate - result.RequestedRate) > eps ||
                Math.Abs(result.MaxAppliedRate - result.RequestedRate) > eps;

            string appliedSegment =
                Math.Abs(result.MinAppliedRate - result.MaxAppliedRate) <= eps
                    ? minApplied                                    
                    : $"{minApplied}–{maxApplied}";                 

            var msg = $"Applied discount {appliedSegment} to products matching “{categoryLike}”.";

            if (adjusted)
            {
                // If policy/affected fields are later exposed, they can be appended here.
                msg += $" (requested {requested}, adjusted by policy)";
            }
                        
            msg += $"  Updated: {result.UpdatedCount}, Discount Adjusted: {result.ClampedCount}.";

            return msg;
        }

        // remove "<categoryLike>"
        var mRemove = RemoveCmd.Match(line);
        if (mRemove.Success)
        {
            var categoryLike = mRemove.Groups[1].Value.Trim();
            var count = await _copilot.RemoveDiscountAsync(categoryLike, ct);
            return $"Removed discounts from {count} product(s) matching “{categoryLike}”.";
        }

        // otherwise: treat as chat
        var reply = await _copilot.ChatAsync(line, ct);
        return reply;
    }

    private static double ParseRate(string text)
    {
        // Accept: "0.30", "30%", "30"
        var hasPercent = text.EndsWith("%", StringComparison.Ordinal);
        var raw = hasPercent ? text[..^1] : text;

        if (!double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            throw new FormatException($"Could not parse discount: “{text}”");

        if (hasPercent) return value / 100.0;

        // If user typed e.g. "30", treat it as 30% (common CLI habit).
        if (value > 1.0) return value / 100.0;

        return value; // already decimal fraction
    }

    private static string ToPercent(double rate) =>
        rate.ToString("P0", CultureInfo.InvariantCulture);
}