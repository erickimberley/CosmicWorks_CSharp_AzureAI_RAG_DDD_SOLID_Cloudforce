using CosmicWorks.Application.Abstractions;

namespace CosmicWorks.Application.UseCases;

// <summary>
/// Application façade that composes use cases into a single interface suitable
/// for the CLI or an HTTP controller layer.
/// </summary>
public sealed class CopilotService : ICopilotService
{
    private readonly ApplyProductDiscount _apply;
    private readonly RemoveProductDiscount _remove;
    private readonly ChatWithCatalog _chat;
    private readonly ListCategories _list;
    private readonly ICatalogPromptBuilder _prompts;

    public CopilotService(
        ApplyProductDiscount apply,
        RemoveProductDiscount remove,
        ChatWithCatalog chat,
        ListCategories list,
        ICatalogPromptBuilder prompts)
    {
        _apply = apply;
        _remove = remove;
        _chat = chat;
        _list = list;
        _prompts = prompts;
    }

    public Task<string> ChatAsync(string prompt, CancellationToken ct = default)
        => _chat.ExecuteAsync(prompt, ct);

    public Task<ApplyDiscountResult> ApplyDiscountAsync(string categoryLike, double rate, CancellationToken ct = default)
        => _apply.ExecuteAsync(categoryLike, rate, ct);

    public Task<int> RemoveDiscountAsync(string categoryLike, CancellationToken ct = default)
        => _remove.ExecuteAsync(categoryLike, ct);

    public Task<IReadOnlyList<string>> ListCategoriesAsync(CancellationToken ct = default)
        => _list.ExecuteAsync(ct);

    public string BuildBasePrompt() => _prompts.BuildBasePrompt();
}