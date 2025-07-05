using SpireCore.AI.Interactions.Contracts;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace SpireCore.AI.Clients;

public abstract class AIClient : IAiClient
{
    protected readonly HttpClient _httpClient;
    protected readonly AiClientConfiguration _settings;

    protected AIClient(HttpClient httpClient, AiClientConfiguration settings)
    {
        _httpClient = httpClient;
        _settings = settings;
        _httpClient.BaseAddress ??= new Uri(_settings.Endpoint);
    }

    protected virtual HttpRequestMessage CreatePostRequest(string path, object payload)
    {
        var json = JsonSerializer.Serialize(payload);
        var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        ApplyHeaders(request);
        return request;
    }

    protected virtual void ApplyHeaders(HttpRequestMessage request)
    {
        // No-op for Ollama, but override in child class if needed (e.g. Bearer token for OpenAI)
    }

    public virtual async Task<HttpResponseMessage> PostAsync(string path, object payload, CancellationToken cancellationToken = default)
    {
        var request = CreatePostRequest(path, payload);
        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new AIClientException(
                $"Provider error: {response.StatusCode}",
                response.StatusCode,
                content
            );
        }

        return response;
    }

    public virtual async IAsyncEnumerable<string> PostStreamAsync(string path, object payload, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = CreatePostRequest(path, payload);
        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new AIClientException(
                $"Provider error: {response.StatusCode}",
                response.StatusCode,
                content
            );
        }

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync();
            if (!string.IsNullOrWhiteSpace(line))
            {
                yield return line;
            }
        }
    }

    /// <summary>
    /// Must be implemented by providers to process a full AI interaction request.
    /// </summary>
    /// <param name="request">Wrapped interaction request including stream/think/options.</param>
    /// <returns>Resulting assistant interaction.</returns>
    public abstract Task<IInteraction> ProcessInteraction(IInteractionRequest request);
}
