
namespace SpireCore.AI.Clients;

public class AiClientConfiguration
{
    /// <summary>
    /// Unique provider key (e.g. "ollama", "openai", "azure", etc).
    /// </summary>
    public string Provider { get; set; } = "";

    /// <summary>
    /// Optional display name for this client/profile.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// API endpoint, if applicable.
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// API Key or access token, if required.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Model names per generation type (text, image, audio, etc).
    /// </summary>
    public Dictionary<string, string> Models { get; set; } = new();

    /// <summary>
    /// Extra provider-specific settings (optional).
    /// </summary>
    public Dictionary<string, object> Extra { get; set; } = new();
}
