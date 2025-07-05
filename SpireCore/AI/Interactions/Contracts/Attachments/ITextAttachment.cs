// ITextAttachment.cs


// ITextAttachment.cs

namespace SpireCore.AI.Interactions.Contracts.Attachments;

/// <summary>
/// Text content attachment.
/// </summary>
public interface ITextAttachment : IInteractionAttachment
{
    string Text { get; }
}
