using SpireCore.AI.Interactions.Attachments;

namespace SpireCore.AI.Interactions.Implementations;

/// <summary>
/// One AI message, file, image, or audio exchange.
/// </summary>
public class Interaction : IInteraction
{
    public string Role { get; }
    public string Type { get; }
    public IReadOnlyList<IInteractionAttachment> Attachments { get; }

    public Interaction(string role, string type, IReadOnlyList<IInteractionAttachment> attachments)
    {
        Role = role;
        Type = type;
        Attachments = attachments;
    }
}
