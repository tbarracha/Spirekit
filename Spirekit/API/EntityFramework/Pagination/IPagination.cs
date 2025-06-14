// -----------------------------------------------------------------------------
// Author: Tiago Barracha <ti.barracha@gmail.com>
// Created with AI assistance (ChatGPT)
// -----------------------------------------------------------------------------

using Spirekit.Core.Constants;
using System.Linq.Expressions;

namespace Spirekit.API.EntityFramework.Pagination;

/// <summary>
/// Defines pagination operations for entities.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public interface IPagination<T> where T : class
{
    /// <summary>
    /// Paged query for all entities matching a given state flag (default: ACTIVE).
    /// </summary>
    Task<PaginatedResult<T>> ListPagedAsync(int page, int pageSize, string state = StateFlags.ACTIVE);

    /// <summary>
    /// Paged filtered query for entities matching a given state flag (default: ACTIVE).
    /// </summary>
    Task<PaginatedResult<T>> ListPagedFilteredAsync(
        Expression<Func<T, bool>> filter,
        int page,
        int pageSize,
        string state = StateFlags.ACTIVE);
}
