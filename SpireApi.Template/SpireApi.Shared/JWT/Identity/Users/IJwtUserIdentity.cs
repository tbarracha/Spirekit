using SpireCore.Abstractions.Interfaces;

namespace SpireApi.Shared.JWT.Identity.Users;

public interface IJwtUserIdentity : IHasId<Guid>
{
    string? Email { get; }
    string? UserName { get; }
    string? DisplayName { get; }

    string? FirstName { get; }
    string? LastName { get; }
}