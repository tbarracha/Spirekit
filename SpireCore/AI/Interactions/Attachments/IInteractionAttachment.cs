// IInteractionAttachment.cs

namespace SpireCore.AI.Interactions.Attachments;

/// <summary>
/// An attachment or payload associated with an interaction (e.g. text, image, file, audio).
/// </summary>
public interface IInteractionAttachment
{
    /// <summary>
    /// The type of this attachment (see AiInteractionTypes).
    /// </summary>
    string Type { get; }

    /// <summary>
    /// (Optional) Short description, filename, or summary.
    /// </summary>
    string? Label { get; }
}
