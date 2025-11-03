using CosmicWorks.Domain.Repositories;

namespace CosmicWorks.Application.UseCases;

/// <summary>
/// Simple query use case that exposes distinct category names.
/// </summary>
public sealed class ListCategories
{
    private readonly IProductReader _reader;

    public ListCategories(IProductReader reader) => _reader = reader;

    public Task<IReadOnlyList<string>> ExecuteAsync(CancellationToken ct = default)
        => _reader.ListCategoriesAsync(ct);
}