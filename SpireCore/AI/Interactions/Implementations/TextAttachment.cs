namespace SpireCore.AI.Interactions.Attachments;

/// <summary>
/// Text content attachment.
/// </summary>
public class TextAttachment : ITextAttachment
{
    public string Type => "text";
    public string? Label { get; }
    public string Text { get; }

    public TextAttachment(string text, string? label = null)
    {
        Text = text;
        Label = label;
    }
}
