using CosmicWorks.Application.Abstractions;
using CosmicWorks.Application.UseCases;

namespace CosmicWorks.Tests.Testing.CLI;

internal sealed class NoopCopilot : ICopilotService
{
    public Task<string> ChatAsync(string p, CancellationToken ct = default) => Task.FromResult("ok");
    
    public Task<IReadOnlyList<string>> ListCategoriesAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
    
    public Task<ApplyDiscountResult> ApplyDiscountAsync(string c, double r, CancellationToken ct = default) =>
        Task.FromResult(new ApplyDiscountResult(0, 0, r, r, r));
    
    public Task<int> RemoveDiscountAsync(string c, CancellationToken ct = default) => Task.FromResult(0);
    
    public string BuildBasePrompt() => "SYS|demo";
}