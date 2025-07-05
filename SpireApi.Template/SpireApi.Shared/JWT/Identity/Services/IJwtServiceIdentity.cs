using SpireCore.Abstractions.Interfaces;

namespace SpireApi.Shared.JWT.Identity.Services;

public interface IJwtServiceIdentity : IHasId<Guid>, IStateFlag
{
    string ServiceName { get; }
    string? Description { get; }

    DateTime RegisteredAt { get; }
    DateTime? LastUsedAt { get; }
    string? LastUsedIp { get; }

    // API key (optional if you want to support JWTs only)
    string ApiKeyHash { get; }
    string? ApiKeyHint { get; }
    DateTime? ApiKeyLastRotatedAt { get; }

    // Optional advanced
    DateTime? TokenExpiresAt { get; }
    string? TokenVersion { get; }
    List<string>? AllowedScopes { get; }
}
