using CosmicWorks.Application.Abstractions;
using CosmicWorks.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace CosmicWorks.Infrastructure.Integrations.OpenAI;

/// <summary>
/// Azure OpenAI chat adapter implementing Application.Abstractions.IChatService.
/// Builds a Chat Completions request and returns the first choice's content.
/// </summary>
internal sealed class AzureOpenAIChatService : IChatService
{
    private readonly HttpClient _http;
    private readonly OpenAISettings _cfg;
    private readonly Func<Task<string>> _getToken;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public AzureOpenAIChatService(HttpClient http, OpenAISettings cfg)
    {
        _http = http;
        _cfg = cfg;
        
        if (string.IsNullOrWhiteSpace(cfg.ApiKey))
            throw new InvalidOperationException("OpenAI.ApiKey is empty while UseEntraID=false.");
        _getToken = () => Task.FromResult(cfg.ApiKey);        
    }

    public async Task<string> ChatAsync(IReadOnlyList<(string role, string content)> messages, CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post,
            $"{_cfg.Endpoint}/openai/deployments/{_cfg.ChatDeployment}/chat/completions?api-version={_cfg.ApiVersion}");

        var token = await _getToken();        
        req.Headers.Add("api-key", token);

        // map tuple messages to DTOs
        var dto = new ChatRequest
        {
            Messages = messages.Select(m => new ChatMessage { Role = m.role, Content = m.content }).ToList(),
            Temperature = 0.2
        };

        req.Content = JsonContent.Create(dto, options: JsonOpts);
        var res = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
        res.EnsureSuccessStatusCode();

        var body = await res.Content.ReadFromJsonAsync<ChatResponse>(JsonOpts, ct);
        return body?.Choices?.FirstOrDefault()?.Message?.Content ?? "(no content)";
    }
}