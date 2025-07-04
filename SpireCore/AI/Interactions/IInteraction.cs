using SpireCore.AI.Interactions.Attachments;

namespace SpireCore.AI.Interactions;

/// <summary>
/// One AI message, file, image, or audio exchange.
/// </summary>
public interface IInteraction
{
    /// <summary>
    /// The role of the participant (see AiInteractionRoles).
    /// </summary>
    string Role { get; }

    /// <summary>
    /// The type of interaction (see AiInteractionTypes).
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Attachments for this interaction (text, files, images, audio, etc).
    /// </summary>
    IReadOnlyList<IInteractionAttachment> Attachments { get; }
}