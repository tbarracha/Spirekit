// -----------------------------------------------------------------------------
// Author: Tiago Barracha <ti.barracha@gmail.com>
// Created with AI assistance (ChatGPT)
// -----------------------------------------------------------------------------

namespace Spirekit.API.EntityFramework.Repositories;

/// <summary>
/// Defines basic asynchronous CRUD operations for a data repository, including batch support and pagination.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
/// <typeparam name="TId">Primary key type.</typeparam>
public interface IRepository<T, TId> where T : class
{
    // --- Read ---

    /// <summary>Gets an entity by its ID.</summary>
    Task<T?> GetByIdAsync(TId id);

    /// <summary>Gets a list of all active entities.</summary>
    Task<IReadOnlyList<T>> ListAsync();



    // --- Create ---

    /// <summary>Adds a new entity.</summary>
    Task<T> AddAsync(T entity);

    /// <summary>Adds multiple entities.</summary>
    Task<IReadOnlyList<T>> AddRangeAsync(IEnumerable<T> entities);



    // --- Update ---

    /// <summary>Updates an existing entity.</summary>
    Task<T> UpdateAsync(T entity);

    /// <summary>Updates multiple entities.</summary>
    Task<IReadOnlyList<T>> UpdateRangeAsync(IEnumerable<T> entities);



    // --- Delete ---

    /// <summary>Deletes an entity by marking its state as deleted.</summary>
    Task<T> DeleteAsync(T entity);

    /// <summary>Deletes multiple entities by marking their state as deleted.</summary>
    Task<IReadOnlyList<T>> DeleteRangeAsync(IEnumerable<T> entities);

    /// <summary>Soft-deletes an entity by ID. Returns null if not found.</summary>
    Task<T?> SoftDeleteAsync(TId id);
}
