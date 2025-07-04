// ITextAttachment.cs

namespace SpireCore.AI.Interactions.Attachments;

/// <summary>
/// Text content attachment.
/// </summary>
public interface ITextAttachment : IInteractionAttachment
{
    string Text { get; }
}
