using SpireCore.Abstractions.Interfaces;

namespace SpireCore.API.JWT.MicroServiceIdentity;

public interface IJwtServiceAuth : IHasId<Guid>
{
    string ServiceName { get; }
    string? Description { get; }
}