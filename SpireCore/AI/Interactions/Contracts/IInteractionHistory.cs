// IInteractionHistory.cs

namespace SpireCore.AI.Interactions.Contracts;

/// <summary>
/// An ordered history of all interactions in a session.
/// </summary>
public interface IInteractionHistory
{
    /// <summary>
    /// The session/conversation ID (optional).
    /// </summary>
    string? SessionId { get; }

    /// <summary>
    /// All interactions, oldest first.
    /// </summary>
    IReadOnlyList<IInteraction> Interactions { get; }

    /// <summary>
    /// Adds an interaction.
    /// </summary>
    void AddInteraction(IInteraction interaction);

    /// <summary>
    /// Get the last N interactions, optionally filtered by role.
    /// </summary>
    IReadOnlyList<IInteraction> GetLast(int count, params string[]? roles);
}
