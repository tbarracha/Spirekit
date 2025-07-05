namespace SpireCore.AI.Interactions.Contracts;

/// <summary>
/// Represents a high-level AI interaction request, wrapping context + options.
/// </summary>
public interface IInteractionRequest
{
    /// <summary>
    /// The actual interaction message to send (e.g. the user or system message).
    /// </summary>
    IInteraction Interaction { get; }

    /// <summary>
    /// If true, the client should stream partial results (if supported).
    /// </summary>
    bool Stream { get; }

    /// <summary>
    /// Optional flag to enable "thinking mode" if supported by model (e.g., Qwen).
    /// </summary>
    bool Think { get; }

    /// <summary>
    /// Optional custom metadata (temperature, top_p, system tags, etc.).
    /// </summary>
    IDictionary<string, object>? Options { get; }
}
