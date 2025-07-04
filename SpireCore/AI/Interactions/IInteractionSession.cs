// IInteractionSession.cs

using SpireCore.AI.Clients;

namespace SpireCore.AI.Interactions;

/// <summary>
/// Represents a single AI interaction session, holding model info, client, and history.
/// </summary>
public interface IInteractionSession
{
    /// <summary>
    /// The unique session ID for this interaction session.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// The full configuration for the AI client (provider, models, API key, etc).
    /// </summary>
    AiClientConfiguration Configuration { get; }

    /// <summary>
    /// The associated AI client for sending requests.
    /// </summary>
    IAiClient Client { get; }

    /// <summary>
    /// The ordered interaction history for this session.
    /// </summary>
    IInteractionHistory History { get; }

    /// <summary>
    /// Gets the model name for a given interaction type (e.g. text, image, audio).
    /// </summary>
    /// <param name="interactionType">The type of interaction (e.g. "text").</param>
    /// <returns>The configured model name for the interaction type, or a fallback/default.</returns>
    string GetModelForType(string interactionType);

    /// <summary>
    /// Adds a user interaction/message to the session history.
    /// </summary>
    /// <param name="interaction">The user interaction/message.</param>
    void AddUserInteraction(IInteraction interaction);

    /// <summary>
    /// Adds an assistant interaction/message to the session history.
    /// </summary>
    /// <param name="interaction">The assistant interaction/message.</param>
    void AddAssistantInteraction(IInteraction interaction);

    /// <summary>
    /// Gets the most recent interaction in the session, or null if empty.
    /// </summary>
    /// <returns>The last interaction, or null if none.</returns>
    IInteraction? GetLastInteraction();
}
