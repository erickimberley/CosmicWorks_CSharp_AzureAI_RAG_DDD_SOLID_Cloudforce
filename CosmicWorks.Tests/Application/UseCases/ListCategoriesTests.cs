using CosmicWorks.Application.UseCases;
using CosmicWorks.Tests.Testing.Application;
using FluentAssertions;
using Xunit;

namespace CosmicWorks.Tests.Application.UseCases;

public class ListCategoriesTests
{
    [Fact]
    public async Task Delegates_To_Repository()
    {
        var repo = new CapturingRepo();
        var uc = new ListCategories(repo);
        var result = await uc.ExecuteAsync();
        result.Should().Contain("Bikes");
        repo.Calls.Should().Be(1);
    }
}