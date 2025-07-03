using SpireCore.Abstractions.Interfaces;

namespace SpireApi.Shared.JWT.MicroServiceIdentity;

public interface IJwtServiceAuth : IHasId<Guid>
{
    string ServiceName { get; }
    string? Description { get; }
}