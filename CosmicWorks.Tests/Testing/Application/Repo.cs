using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Repositories;

internal sealed class Repo : IProductRepository
{
    public readonly List<Product> Items = new();
    public readonly List<Product> Saved = new();

    public Task<IReadOnlyList<Product>> FindByCategoryAsync(string categoryLike, CancellationToken ct = default)
        => Task.FromResult((IReadOnlyList<Product>)Items.ToList());

    public Task<IReadOnlyList<string>> ListCategoriesAsync(CancellationToken ct = default)
        => Task.FromResult((IReadOnlyList<string>)new[] { "Bikes", "Accessories, Helmets" });

    public Task SaveAsync(Product product, CancellationToken ct = default)
    { Saved.Add(product); return Task.CompletedTask; }
}