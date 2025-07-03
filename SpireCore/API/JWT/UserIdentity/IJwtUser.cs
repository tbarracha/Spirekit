using SpireCore.Abstractions.Interfaces;

namespace SpireCore.API.JWT.UserIdentity;

public interface IJwtUser : IHasId<Guid>
{
    string? UserName { get; }
    string? DisplayName { get; }
    string? Email { get; }
}