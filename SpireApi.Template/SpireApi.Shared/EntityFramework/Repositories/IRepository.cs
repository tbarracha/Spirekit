using SpireApi.Shared.EntityFramework.Repositories;
using SpireCore.Constants;
using System.Linq.Expressions;

/// <summary>
/// Generic repository interface for CRUD operations, with optional pagination.
/// </summary>
public interface IRepository<T, TId> where T : class, IRepoEntity<TId>
{


    // --- Read ---

    Task<T?> GetByIdAsync(TId id, string? state = StateFlags.ACTIVE);

    Task<T?> GetFilteredAsync(Expression<Func<T, bool>> predicate, string? state = StateFlags.ACTIVE);

    Task<IReadOnlyList<T>> ListFilteredAsync(Expression<Func<T, bool>> filter, string? state = StateFlags.ACTIVE);

    Task<IReadOnlyList<T>> ListAsync(string? state = StateFlags.ACTIVE);



    // --- Create ---

    Task<T> AddAsync(T entity);

    Task<IReadOnlyList<T>> AddRangeAsync(IEnumerable<T> entities);



    // --- Update ---

    Task<T> UpdateAsync(T entity);

    Task<IReadOnlyList<T>> UpdateRangeAsync(IEnumerable<T> entities);

    // --- Delete ---

    Task<T> DeleteAsync(T entity);

    Task<IReadOnlyList<T>> DeleteRangeAsync(IEnumerable<T> entities);

    Task<T?> SoftDeleteAsync(TId id);

    Task<T?> RestoreAsync(TId id);
}
