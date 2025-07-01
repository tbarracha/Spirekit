using SpireCore.Events.Dispatcher;

namespace SpireApi.Application.Events.Authentication;

public record AppUserLoggedInEvent(
    Guid UserId,
    string Email,
    DateTime LoggedInAt,
    string? IpAddress = null,
    string? UserAgent = null
) : IDomainEvent;
