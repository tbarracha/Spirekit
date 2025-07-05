using SpireCore.AI.Interactions.Contracts.Attachments;

namespace SpireCore.AI.Interactions.Implementations.Attachments;

public class TextAttachment : InteractionAttachment, ITextAttachment
{
    public string Text { get; }

    public TextAttachment(string text, string? label = null)
        : base("text", label)
    {
        Text = text;
    }
}
