using SpireCore.Constants;

namespace SpireApi.Application.Shared.Entities;

public interface IGuidEntityByRepository<T> : IGuidEntityRepository<T> where T : GuidEntityBy
{
    // --- Audit Queries ---

    /// <summary>
    /// Gets all entities created by the specified user.
    /// </summary>
    Task<IReadOnlyList<T>> ListCreatedByAsync(string createdBy, string? state = StateFlags.ACTIVE);

    /// <summary>
    /// Gets all entities last updated by the specified user.
    /// </summary>
    Task<IReadOnlyList<T>> ListUpdatedByAsync(string updatedBy, string? state = StateFlags.ACTIVE);

    /// <summary>
    /// Gets a single entity created by the specified user (if any).
    /// </summary>
    Task<T?> GetCreatedByAsync(string createdBy, string? state = StateFlags.ACTIVE);

    /// <summary>
    /// Gets a single entity last updated by the specified user (if any).
    /// </summary>
    Task<T?> GetUpdatedByAsync(string updatedBy, string? state = StateFlags.ACTIVE);
}
