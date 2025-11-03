using CosmicWorks.Application.Abstractions;
using CosmicWorks.Domain.Entities;
using System.Globalization;

namespace CosmicWorks.Application.Prompting;

/// <summary>
/// Builds the base system prompt and an inline product context summary
/// that the chat service uses to ground answers.
/// </summary>
public sealed class CatalogPromptBuilder : ISystemPromptTemplate, ICatalogContextPrompt, ICatalogPromptBuilder
{
    public string BuildSystemPrompt(IReadOnlyList<Product> items)
        => BuildBasePrompt() + "\nContext:\n" + BuildContext(items);

    // TODO: Read in the base prompt from a file or resource instead of hardcoding it here.
    // Use the configuration appsettings to specify the path to the file
    public string BuildBasePrompt() => """
You are an intelligent copilot for Cosmic Works designed to help users manage and find bicycle-related products.
You are helpful, friendly, and knowledgeable, but can only answer questions about Cosmic Works products.
If asked to apply a discount:
  – Apply the specified discount to all products in the specified category. If the user omitted either
    the discount (decimal or percent) or the category, ask for the missing info.
If asked to remove discounts from a category:
  – Set discount=0 for products in that category.
When asked for product lists:
  – Provide at least 3 candidate products unless the user requests a different count.
  – Include name, description, price, SKU, and if discounted, show % and sale price.
  - You must always include the sale_price field value if a product has a discount field value greater than 0.
""";

    private static string BuildContext(IReadOnlyList<Product> items) =>
        string.Join('\n', items.Select(p =>
            $"{p.Name} | {p.Description} | {p.Sku} | {p.Price.Amount.ToString("0.##", CultureInfo.InvariantCulture)}"));
}