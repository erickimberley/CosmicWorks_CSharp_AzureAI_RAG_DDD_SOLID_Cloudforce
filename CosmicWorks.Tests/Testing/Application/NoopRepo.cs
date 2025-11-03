using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Repositories;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class NoopRepo : IProductRepository
{
    public Task<IReadOnlyList<Product>> FindByCategoryAsync(string categoryLike, CancellationToken ct = default)
        => Task.FromResult((IReadOnlyList<Product>)Array.Empty<Product>());
    public Task<IReadOnlyList<string>> ListCategoriesAsync(CancellationToken ct = default)
        => Task.FromResult((IReadOnlyList<string>)Array.Empty<string>());
    public Task SaveAsync(Product product, CancellationToken ct = default) => Task.CompletedTask;
}