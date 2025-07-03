using SpireCore.Abstractions.Interfaces;

namespace {Namespace}.Shared.JWT.UserIdentity;

public interface IJwtUser : IHasId<Guid>
{
    string? UserName { get; }
    string? DisplayName { get; }
    string? Email { get; }
}
