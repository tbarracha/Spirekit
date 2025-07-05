using SpireCore.AI.Clients;
using SpireCore.AI.Interactions.Contracts;

namespace SpireCore.AI.Interactions.Implementations;

public class InteractionSession : IInteractionSession
{
    public string Id { get; }
    public AiClientConfiguration Configuration { get; }
    public IAiClient Client { get; }
    public IInteractionHistory History { get; }

    public InteractionSession(string id, AiClientConfiguration config, IAiClient client)
    {
        Id = id;
        Configuration = config;
        Client = client;
        History = new InteractionHistory(id);
    }

    public string GetModelForType(string interactionType)
    {
        return Configuration.Models.TryGetValue(interactionType, out var model)
            ? model
            : Configuration.Models.GetValueOrDefault("text") ?? "";
    }

    public void AddUserInteraction(IInteraction interaction) =>
        History.AddInteraction(interaction);

    public void AddAssistantInteraction(IInteraction interaction) =>
        History.AddInteraction(interaction);

    public IInteraction? GetLastInteraction() =>
        History.Interactions.LastOrDefault();
}
