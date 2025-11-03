namespace CosmicWorks.Domain.Repositories;

/// <summary>
/// Full repository for reading and writing Product aggregates.
/// </summary>
public interface IProductRepository : IProductReader, IProductWriter
{
}