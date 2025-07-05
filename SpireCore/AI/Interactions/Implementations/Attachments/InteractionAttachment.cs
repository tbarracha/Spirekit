using SpireCore.AI.Interactions.Contracts.Attachments;

namespace SpireCore.AI.Interactions.Implementations.Attachments;

public abstract class InteractionAttachment : IInteractionAttachment
{
    public string Type { get; }
    public string? Label { get; }

    protected InteractionAttachment(string type, string? label = null)
    {
        Type = type;
        Label = label;
    }
}
