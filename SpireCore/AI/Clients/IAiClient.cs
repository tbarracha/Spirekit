using SpireCore.AI.Interactions;

namespace SpireCore.AI.Clients;

public interface IAiClient
{
    Task<IInteraction> ProcessInteraction(IInteraction interactionRequest);
}