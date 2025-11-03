using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.Repositories;

namespace CosmicWorks.Tests.Testing.Application;

internal sealed class CapturingRepo : IProductRepository
{
    public int Calls;
    
    public Task<IReadOnlyList<Product>> FindByCategoryAsync(string categoryLike, CancellationToken ct = default)
        => Task.FromResult((IReadOnlyList<Product>)Array.Empty<Product>());

    public Task<IReadOnlyList<string>> ListCategoriesAsync(CancellationToken ct = default)
    { Calls++; return Task.FromResult((IReadOnlyList<string>)new[] { "Bikes" }); }

    public Task SaveAsync(Product product, CancellationToken ct = default) => Task.CompletedTask;
}