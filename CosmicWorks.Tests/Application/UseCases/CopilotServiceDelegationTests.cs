using CosmicWorks.Application.UseCases;
using CosmicWorks.Domain.Entities;
using CosmicWorks.Domain.ValueObjects;
using CosmicWorks.Tests.Testing.Application;
using FluentAssertions;
using Xunit;

namespace CosmicWorks.Tests.Application.UseCases;

public class CopilotServiceDelegationTests
{
    private static Product P(decimal price = 100m, double discount = 0.0, string id = "p")
    {
        var p = new Product(new ProductId(id), new CategoryId("c"), "Accessories, Helmets", "Helmet", "HL-1", "desc", new Money(price));
        if (discount > 0) p.ApplyDiscount(DiscountRate.Create(discount));
        return p;
    }

    [Fact]
    public async Task ApplyDiscountAsync_Delegates_To_UseCase_And_Returns_Result()
    {
        var repo = new Repo(); repo.Items.Add(P());
        var apply = new ApplyProductDiscount(repo, new NoopDispatcher(), new Policy(0.30));
        var remove = new RemoveProductDiscount(repo, repo, new NoopDispatcher());
        var chatUc = new ChatWithCatalog(new Search(), new Embed(), new Chat(), new Prompts(), 3, 0.2);
        var list = new ListCategories(repo);
        var svc = new CopilotService(apply, remove, chatUc, list, new Prompts());

        var r = await svc.ApplyDiscountAsync("helmets", 0.20);

        r.UpdatedCount.Should().Be(1);
        r.ClampedCount.Should().Be(0);
    }

    [Fact]
    public async Task RemoveDiscountAsync_Delegates_And_Skips_Zero()
    {
        var repo = new Repo(); repo.Items.Add(P(discount: 0.15, id: "p1")); repo.Items.Add(P(id: "p2"));
        var apply = new ApplyProductDiscount(repo, new NoopDispatcher(), new Policy(0.30));
        var remove = new RemoveProductDiscount(repo, repo, new NoopDispatcher());
        var chatUc = new ChatWithCatalog(new Search(), new Embed(), new Chat(), new Prompts(), 3, 0.2);
        var list = new ListCategories(repo);
        var svc = new CopilotService(apply, remove, chatUc, list, new Prompts());

        var updated = await svc.RemoveDiscountAsync("helmets");
        updated.Should().Be(1);
        repo.Saved.Should().HaveCount(1);
        repo.Saved[0].Discount.Value.Should().Be(0.0);
    }

    [Fact]
    public async Task ChatAsync_Delegates_And_Composes_Messages()
    {
        var repo = new Repo();
        var apply = new ApplyProductDiscount(repo, new NoopDispatcher(), new Policy(0.30));
        var remove = new RemoveProductDiscount(repo, repo, new NoopDispatcher());
        var search = new Search(); var embed = new Embed(); var chat = new Chat(); var prompts = new Prompts();
        var chatUc = new ChatWithCatalog(search, embed, chat, prompts, topK: 4, minSimilarity: 0.1);
        var list = new ListCategories(repo);
        var svc = new CopilotService(apply, remove, chatUc, list, prompts);

        var reply = await svc.ChatAsync("find helmets");

        reply.Should().Be("OK");
        embed.Last.Should().Be("find helmets");
        search.LastK.Should().Be(4);
        search.LastMin.Should().Be(0.1);
        chat.Last![0].role.Should().Be("system");
        chat.Last![1].content.Should().Be("find helmets");
    }

    [Fact]
    public async Task ListCategoriesAsync_Delegates_Directly()
    {
        var repo = new Repo();
        var svc = new CopilotService(
            new ApplyProductDiscount(repo, new NoopDispatcher(), new Policy(0.30)),
            new RemoveProductDiscount(repo, repo, new NoopDispatcher()),
            new ChatWithCatalog(new Search(), new Embed(), new Chat(), new Prompts(), 3, 0.2),
            new ListCategories(repo),
            new Prompts());

        var cats = await svc.ListCategoriesAsync();
        cats.Should().Contain(new[] { "Bikes", "Accessories, Helmets" });
    }
}