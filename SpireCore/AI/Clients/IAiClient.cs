using SpireCore.AI.Interactions.Contracts;

namespace SpireCore.AI.Clients;

public interface IAiClient
{
    Task<IInteraction> ProcessInteraction(IInteractionRequest request);
}
