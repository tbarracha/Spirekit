using SpireCore.AI.Interactions.Contracts;

namespace SpireCore.AI.Interactions.Implementations;

public class InteractionHistory : IInteractionHistory
{
    public string? SessionId { get; }
    private readonly List<IInteraction> _interactions = new();

    public IReadOnlyList<IInteraction> Interactions => _interactions;

    public InteractionHistory(string? sessionId)
    {
        SessionId = sessionId;
    }

    public void AddInteraction(IInteraction interaction)
    {
        _interactions.Add(interaction);
    }

    public IReadOnlyList<IInteraction> GetLast(int count, params string[]? roles)
    {
        var query = _interactions.AsEnumerable();
        if (roles is { Length: > 0 })
            query = query.Where(i => roles.Contains(i.Role));
        return query.Reverse().Take(count).Reverse().ToList();
    }
}
