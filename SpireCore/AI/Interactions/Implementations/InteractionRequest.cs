using SpireCore.AI.Interactions.Contracts;

namespace SpireCore.AI.Interactions.Implementations;

public class InteractionRequest : IInteractionRequest
{
    public IInteraction Interaction { get; }
    public bool Stream { get; }
    public bool Think { get; }
    public IDictionary<string, object>? Options { get; }

    public InteractionRequest(
        IInteraction interaction,
        bool stream = false,
        bool think = false,
        IDictionary<string, object>? options = null)
    {
        Interaction = interaction;
        Stream = stream;
        Think = think;
        Options = options;
    }
}
