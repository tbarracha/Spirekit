using SpireApi.Shared.EntityFramework.Entities.Abstractions;
using SpireCore.Constants;
using System.Linq.Expressions;

namespace SpireApi.Shared.EntityFramework.Repositories;

public interface IAuditableEntityRepository<T, TId> : IEntityRepository<T, TId>
    where T : class, IAuditableEntity<TId>
{
    // --- Read ---

    Task<T?> GetByIdAsync(TId id, string actor, string? state = StateFlags.ACTIVE);

    Task<T?> GetFilteredAsync(Expression<Func<T, bool>> predicate, string actor, string? state = StateFlags.ACTIVE);

    Task<IReadOnlyList<T>> ListFilteredAsync(Expression<Func<T, bool>> filter, string actor, string? state = StateFlags.ACTIVE);

    Task<IReadOnlyList<T>> ListAsync(string actor, string? state = StateFlags.ACTIVE);

    // --- Create ---

    Task<T> AddAsync(T entity, string actor);

    Task<IReadOnlyList<T>> AddRangeAsync(IEnumerable<T> entities, string actor);

    // --- Update ---

    Task<T> UpdateAsync(T entity, string actor);

    Task<IReadOnlyList<T>> UpdateRangeAsync(IEnumerable<T> entities, string actor);

    // --- Delete ---

    Task<T> DeleteAsync(T entity, string actor);

    Task<IReadOnlyList<T>> DeleteRangeAsync(IEnumerable<T> entities, string actor);

    Task<T?> SoftDeleteAsync(TId id, string actor);

    Task<T?> RestoreAsync(TId id, string actor);
}
