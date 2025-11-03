using CosmicWorks.Application.Abstractions;
using CosmicWorks.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace CosmicWorks.Infrastructure.Integrations.OpenAI;

/// <summary>
/// Azure OpenAI embeddings adapter implementing Application.Abstractions.IEmbeddingService.
/// </summary>
internal sealed class AzureOpenAIEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _http;
    private readonly OpenAISettings _cfg;
    private readonly Func<Task<string>> _getToken;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public AzureOpenAIEmbeddingService(HttpClient http, OpenAISettings cfg)
    {
        _http = http;
        _cfg = cfg;
        
        if (string.IsNullOrWhiteSpace(cfg.ApiKey))
            throw new InvalidOperationException("OpenAI.ApiKey is empty.");
        _getToken = () => Task.FromResult(cfg.ApiKey);       
    }

    public async Task<float[]> EmbedAsync(string text, CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post,
            $"{_cfg.Endpoint}/openai/deployments/{_cfg.EmbeddingDeployment}/embeddings?api-version={_cfg.ApiVersion}");

        var token = await _getToken();        
        req.Headers.Add("api-key", token);

        req.Content = JsonContent.Create(new EmbeddingsRequest { Input = text });
        var res = await _http.SendAsync(req, ct);
        res.EnsureSuccessStatusCode();

        var body = await res.Content.ReadFromJsonAsync<EmbeddingsResponse>(JsonOpts, ct);
        return (body?.Data?.FirstOrDefault()?.Embedding?.ToArray()) ?? Array.Empty<float>();
    }
}