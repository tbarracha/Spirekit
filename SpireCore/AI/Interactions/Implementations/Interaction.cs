using SpireCore.AI.Interactions.Contracts;
using SpireCore.AI.Interactions.Contracts.Attachments;

namespace SpireCore.AI.Interactions.Implementations;

public class Interaction : IInteraction
{
    public string Role { get; }
    public string Type { get; }
    public IReadOnlyList<IInteractionAttachment> Attachments { get; }

    public Interaction(string role, string type, IEnumerable<IInteractionAttachment> attachments)
    {
        Role = role;
        Type = type;
        Attachments = attachments.ToList();
    }
}
