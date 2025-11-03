using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Repositories;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class FakeReaderWriter : IProductReader, IProductWriter
{
    public List<Product> Items { get; } = new();
    public readonly List<Product> Saved = new();

    public Task<IReadOnlyList<Product>> FindByCategoryAsync(string categoryLike, CancellationToken ct = default)
        => Task.FromResult((IReadOnlyList<Product>)Items.ToList());

    public Task<IReadOnlyList<string>> ListCategoriesAsync(CancellationToken ct = default)
        => Task.FromResult((IReadOnlyList<string>)new List<string>());

    public Task SaveAsync(Product product, CancellationToken ct = default)
    {
        Saved.Add(product);
        return Task.CompletedTask;
    }
}
