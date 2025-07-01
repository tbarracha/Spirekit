using SpireCore.Events.Dispatcher;

namespace SpireApi.Application.Events.Authentication;

public record AppUserRegisteredEvent(
    Guid UserId,
    string Email,
    DateTime RegisteredAt
) : IDomainEvent;
