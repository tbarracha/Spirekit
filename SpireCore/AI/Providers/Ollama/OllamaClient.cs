// SpireCore.AI.Providers.Ollama

using System.Text.Json;
using SpireCore.AI.Clients;
using SpireCore.AI.Interactions;
using SpireCore.AI.Interactions.Attachments;
using SpireCore.AI.Interactions.Implementations;

namespace SpireCore.AI.Providers.Ollama;

public class OllamaClient : AIClient
{
    public OllamaClient(HttpClient httpClient, AiClientConfiguration config)
        : base(httpClient, config) { }

    // Map Interaction -> OllamaGenerateRequest
    private OllamaGenerateRequest MapInteractionToOllamaRequest(IInteraction interaction)
    {
        var text = interaction.Attachments.OfType<ITextAttachment>().FirstOrDefault()?.Text ?? "";

        // Map multimodal images (base64) if present
        var images = interaction.Attachments
            .OfType<IFileAttachment>()
            .Where(f => f.MimeType.StartsWith("image/"))
            .Select(f => f.Base64ContentsOrUrl)
            .ToList();

        // Advanced options (temperature, stop, etc.)
        var options = _settings.Extra?.ToDictionary(kv => kv.Key, kv => kv.Value);

        return new OllamaGenerateRequest
        {
            Model = _settings.Models.TryGetValue(interaction.Type, out var m) ? m : _settings.Models.GetValueOrDefault("text") ?? "llama3",
            Prompt = text,
            Images = images.Count > 0 ? images : null,
            Stream = true, // Default to streaming
            Options = options
        };
    }

    // Map OllamaGenerateResponse -> Interaction
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

    // ProcessInteraction: Streaming or non-streaming response
    public override async Task<IInteraction> ProcessInteraction(IInteraction interactionRequest)
    {
        var payload = MapInteractionToOllamaRequest(interactionRequest);

        // Determine streaming based on Extra/config (default: true)
        bool streaming = true;
        if (_settings.Extra.TryGetValue("stream", out var streamObj) && streamObj is bool stream)
            streaming = stream;

        if (!streaming)
        {
            // Non-streaming: read entire response as a single JSON object
            var response = await PostAsync("/api/generate", payload);
            var responseStr = await response.Content.ReadAsStringAsync();
            var respObj = JsonSerializer.Deserialize<OllamaGenerateResponse>(responseStr);
            return MapOllamaResponseToInteraction(respObj!);
        }
        else
        {
            // Streaming: aggregate streamed partial responses
            string fullResponse = "";
            await foreach (var jsonLine in PostStreamAsync("/api/generate", payload))
            {
                if (string.IsNullOrWhiteSpace(jsonLine))
                    continue;

                var resp = JsonSerializer.Deserialize<OllamaGenerateResponse>(jsonLine);
                if (resp != null && !string.IsNullOrEmpty(resp.Response))
                    fullResponse += resp.Response;
                if (resp != null && resp.Done.HasValue && resp.Done.Value)
                    break;
            }

            // Compose a single Interaction for the full reply
            return new Interaction(
                "assistant",
                "text",
                new List<IInteractionAttachment> { new TextAttachment(fullResponse) }
            );
        }
    }


    // Streaming completion as async enumerable
    public async IAsyncEnumerable<IInteraction> StreamInteractionAsync(
        IInteraction interactionRequest,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var payload = MapInteractionToOllamaRequest(interactionRequest);
        payload.Stream = true;

        await foreach (var jsonLine in PostStreamAsync("/api/generate", payload, cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(jsonLine))
                continue;
            var resp = JsonSerializer.Deserialize<OllamaGenerateResponse>(jsonLine);
            if (resp != null && !string.IsNullOrWhiteSpace(resp.Response))
                yield return MapOllamaResponseToInteraction(resp);
        }
    }
}
