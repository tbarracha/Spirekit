﻿// -----------------------------------------------------------------------------
// Author: Tiago Barracha <ti.barracha@gmail.com>
// Created with AI assistance (ChatGPT)
// -----------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using SpireCore.Abstractions.Interfaces;
using SpireCore.Constants;
using System.Linq.Expressions;

namespace SpireCore.API.EntityFramework.Repositories;

/// <summary>
/// Defines basic asynchronous CRUD operations for a data repository, including batch support and pagination.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
/// <typeparam name="TId">Primary key type.</typeparam>
public interface IRepository<T, TId> where T : class, ICreatedAt, IUpdatedAt, IStateFlag
{
    // --- Read ---

    /// <summary>Gets an entity by its ID.</summary>
    Task<T?> GetByIdAsync(TId id);

    /// <summary>Gets a list of all entities matching a given state flag (default: ACTIVE).</summary>
    Task<IReadOnlyList<T>> ListAsync(string state = StateFlags.ACTIVE);

    /// <summary>Gets a filtered list of entities matching a given state flag (default: ACTIVE).</summary>
    Task<IReadOnlyList<T>> ListFilteredAsync(Expression<Func<T, bool>> filter, string state = StateFlags.ACTIVE);



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
