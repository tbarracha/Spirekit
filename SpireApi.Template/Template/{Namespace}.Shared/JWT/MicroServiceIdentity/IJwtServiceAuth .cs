using SpireCore.Abstractions.Interfaces;

namespace {Namespace}.Shared.JWT.MicroServiceIdentity;

public interface IJwtServiceAuth : IHasId<Guid>
{
    string ServiceName { get; }
    string? Description { get; }
}
