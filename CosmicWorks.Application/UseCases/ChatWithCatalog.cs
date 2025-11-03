using CosmicWorks.Application.Abstractions;
using CosmicWorks.Domain.Repositories;

namespace CosmicWorks.Application.UseCases;

/// <summary>
/// RAG-style chat use case that embeds the user input, retrieves nearest product
/// neighbors via vector search, builds a grounded system prompt, and delegates
/// the final completion to IChatService.
/// </summary>
public sealed class ChatWithCatalog
{    
    private readonly IProductSimilaritySearcher _search;
    private readonly IEmbeddingService _embeddings;
    private readonly IChatService _chat;
    private readonly ICatalogPromptBuilder _prompts;
    private readonly int _topK;
    private readonly double _minSimilarity;

    public ChatWithCatalog(
        IProductSimilaritySearcher search,
        IEmbeddingService embeddings,
        IChatService chat,
        ICatalogPromptBuilder prompts,
        int topK,
        double minSimilarity)
    {
        _search = search;
        _embeddings = embeddings;

        _chat = chat;
        _prompts = prompts;
        _topK = topK;
        _minSimilarity = minSimilarity;
    }
        
    public async Task<string> ExecuteAsync(string userInput, CancellationToken ct = default)
    {
        // Embed the user request (align the method name to your IEmbeddingService)
        var vector = await _embeddings.EmbedAsync(userInput, ct);

        // Retrieve similar products        
        var neighbors = await _search.FindSimilarAsync(vector, _topK, _minSimilarity, ct);

        // Build inline context + system prompt        
        var system = _prompts.BuildSystemPrompt(neighbors);

        // Compose messages and call the low-level chat client
        var messages = new List<(string role, string content)>
        {
            ("system", system),
            ("user", userInput)
        };
                
        return await _chat.ChatAsync(messages, ct);
    }      
}