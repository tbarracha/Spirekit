using SpireCore.API.EntityFramework.Entities.Base;
using SpireCore.Constants;
using SpireCore.Lists.Pagination;
using SpireCore.Services;
using System.Linq.Expressions;

namespace SpireCore.API.EntityFramework.Entities.Repositories;

/// <summary>
/// Generic repository interface that defines asynchronous CRUD operations,
/// soft‑delete helpers, and optional pagination for an entity type <typeparamref name="T"/>.
/// Implementations are expected to respect the <see cref="StateFlags"/> model so callers
/// can include or exclude soft‑deleted records via the <paramref name="state"/> parameter.
/// </summary>
/// <typeparam name="T">Entity type handled by the repository.</typeparam>
/// <typeparam name="TId">Type of the entity's primary key.</typeparam>
public interface IEntityRepository<T, TId> : IPagination<T>, ITransientService where T : class, IEntity<TId>
{


    // ────────────── Read ────────────────────────────────────────────────

    /// <summary>
    /// Returns the entity whose <c>Id</c> equals <paramref name="id"/>, or <see langword="null"/>
    /// if not found. Only entities whose <c>StateFlag</c> equals <paramref name="state"/> are considered.
    /// </summary>
    Task<T?> GetByIdAsync(TId id, string? state = StateFlags.ACTIVE);

    /// <summary>
    /// Returns the first entity that matches <paramref name="predicate"/> (and <paramref name="state"/>),
    /// or <see langword="null"/> when none exists.
    /// </summary>
    Task<T?> GetFilteredAsync(Expression<Func<T, bool>> predicate, string? state = StateFlags.ACTIVE);


    /// <summary>
    /// Returns all entities that satisfy <paramref name="filter"/> and have the given <paramref name="state"/>.
    /// </summary>
    Task<IReadOnlyList<T>> ListFilteredAsync(Expression<Func<T, bool>> filter, string? state = StateFlags.ACTIVE);

    /// <summary>
    /// Returns every entity whose <c>StateFlag</c> equals <paramref name="state"/>.
    /// Specify <see langword="null"/> to ignore <c>StateFlag</c> completely.
    /// </summary>
    Task<IReadOnlyList<T>> ListAsync(string? state = StateFlags.ACTIVE);


    /// <summary>
    /// Retrieves the first entity that matches <paramref name="predicate"/> and whose
    /// <c>StateFlag</c> equals <paramref name="state"/>; returns <see langword="null"/>
    /// when no such entity exists.
    /// </summary>
    /// <param name="predicate">Filter condition to apply.</param>
    /// <param name="state">
    /// The required <see cref="StateFlags"/> value; pass <see langword="null"/> to ignore
    /// <c>StateFlag</c> entirely.
    /// </param>
    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        string? state = StateFlags.ACTIVE);


    /// <summary>
    /// Returns <see langword="true"/> if at least one entity matches <paramref name="predicate"/>
    /// and the specified <paramref name="state"/>.
    /// </summary>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, string? state = StateFlags.ACTIVE);

    /// <summary>
    /// Counts the number of entities that satisfy <paramref name="predicate"/> and <paramref name="state"/>.
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, string? state = StateFlags.ACTIVE);



    // ────────────── Create ─────────────────────────────────────────────

    /// <summary>
    /// Persists a new entity and returns it with timestamps populated.
    /// </summary>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Persists a collection of new entities in a single batch and returns the saved instances.
    /// </summary>
    Task<IReadOnlyList<T>> AddRangeAsync(IEnumerable<T> entities);



    // ────────────── Update ─────────────────────────────────────────────

    /// <summary>
    /// Updates an existing entity and refreshes its <c>UpdatedAt</c> timestamp.
    /// </summary>
    Task<T> UpdateAsync(T entity);

    /// <summary>
    /// Updates multiple entities in bulk, setting <c>UpdatedAt</c> on each.
    /// </summary>
    Task<IReadOnlyList<T>> UpdateRangeAsync(IEnumerable<T> entities);



    // ────────────── Delete / Restore ───────────────────────────────────

    /// <summary>
    /// Soft‑deletes an entity instance by setting its <c>StateFlag</c> to <see cref="StateFlags.DELETED"/>.
    /// </summary>
    Task<T> DeleteAsync(T entity);

    /// <summary>
    /// Soft‑deletes a collection of entities in one operation.
    /// </summary>
    Task<IReadOnlyList<T>> DeleteRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Convenience helper that soft‑deletes an entity identified by <paramref name="id"/>.
    /// Returns the updated entity or <see langword="null"/> when the id is not found.
    /// </summary>
    Task<T?> SoftDeleteAsync(TId id);

    /// <summary>
    /// Restores a previously soft‑deleted entity (identified by <paramref name="id"/>) by
    /// setting its <c>StateFlag</c> back to <see cref="StateFlags.ACTIVE"/>.
    /// Returns the updated entity or <see langword="null"/> if the entity is not found.
    /// </summary>
    Task<T?> RestoreAsync(TId id);
}
