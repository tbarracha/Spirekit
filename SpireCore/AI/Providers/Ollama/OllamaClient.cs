using System.Text.Json;
using SpireCore.AI.Clients;
using SpireCore.AI.Interactions.Contracts;
using SpireCore.AI.Interactions.Contracts.Attachments;
using SpireCore.AI.Interactions.Implementations;
using SpireCore.AI.Interactions.Implementations.Attachments;

namespace SpireCore.AI.Providers.Ollama;

public class OllamaClient : AIClient
{
    public OllamaClient(HttpClient httpClient, AiClientConfiguration config)
        : base(httpClient, config) { }

    private OllamaGenerateRequest MapRequestToOllama(IInteractionRequest request)
    {
        var interaction = request.Interaction;
        var text = interaction.Attachments.OfType<ITextAttachment>().FirstOrDefault()?.Text ?? "";

        var images = interaction.Attachments
            .OfType<IFileAttachment>()
            .Where(f => f.MimeType.StartsWith("image/"))
            .Select(f => f.Base64ContentsOrUrl)
            .ToList();

        var model = _settings.Models.TryGetValue(interaction.Type, out var m) ? m : _settings.Models.GetValueOrDefault("text") ?? "llama3";

        // Merge client-level options with request-level options
        var mergedOptions = new Dictionary<string, object>();
        if (_settings.Extra is not null)
        {
            foreach (var kv in _settings.Extra)
                mergedOptions[kv.Key] = kv.Value;
        }

        if (request.Options is not null)
        {
            foreach (var kv in request.Options)
                mergedOptions[kv.Key] = kv.Value;
        }

        return new OllamaGenerateRequest
        {
            Model = model,
            Prompt = text,
            Images = images.Count > 0 ? images : null,
            Stream = request.Stream,
            Options = mergedOptions.Count > 0 ? mergedOptions : null
        };
    }

    private static IInteraction MapOllamaResponseToInteraction(OllamaGenerateResponse resp)
    {
        return new Interaction(
            "assistant",
            "text",
            new List<IInteractionAttachment>
            {
                new TextAttachment(resp.Response)
            }
        );
    }

    public override async Task<IInteraction> ProcessInteraction(IInteractionRequest request)
    {
        var payload = MapRequestToOllama(request);

        if (!request.Stream)
        {
            var response = await PostAsync("/api/generate", payload);
            var responseStr = await response.Content.ReadAsStringAsync();
            var respObj = JsonSerializer.Deserialize<OllamaGenerateResponse>(responseStr);
            return MapOllamaResponseToInteraction(respObj!);
        }
        else
        {
            string fullResponse = "";
            await foreach (var jsonLine in PostStreamAsync("/api/generate", payload))
            {
                if (string.IsNullOrWhiteSpace(jsonLine)) continue;

                var resp = JsonSerializer.Deserialize<OllamaGenerateResponse>(jsonLine);
                if (resp != null)
                    fullResponse += resp.Response;
                if (resp?.Done == true)
                    break;
            }

            return new Interaction(
                "assistant",
                "text",
                new List<IInteractionAttachment> { new TextAttachment(fullResponse) }
            );
        }
    }

    public async IAsyncEnumerable<IInteraction> StreamInteractionAsync(
        IInteractionRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var payload = MapRequestToOllama(request);
        payload.Stream = true;

        await foreach (var jsonLine in PostStreamAsync("/api/generate", payload, cancellationToken))
        {
            var resp = JsonSerializer.Deserialize<OllamaGenerateResponse>(jsonLine);
            if (resp != null && !string.IsNullOrWhiteSpace(resp.Response))
                yield return MapOllamaResponseToInteraction(resp);
        }
    }
}
