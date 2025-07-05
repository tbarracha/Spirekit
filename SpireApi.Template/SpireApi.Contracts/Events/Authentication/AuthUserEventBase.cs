using SpireApi.Shared.JWT.Identity.Users;
using SpireCore.Events.Dispatcher;

namespace SpireApi.Contracts.Events.Authentication;

public abstract class AuthUserEventBase : IJwtUserIdentity, IDomainEvent
{
    public Guid Id { get; set; }
    public Guid AuthUserId { get; set; }

    public string Email { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string? DisplayName { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;

}
