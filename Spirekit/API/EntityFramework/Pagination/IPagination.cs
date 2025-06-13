// -----------------------------------------------------------------------------
// Author: Tiago Barracha <ti.barracha@gmail.com>
// Created with AI assistance (ChatGPT)
// -----------------------------------------------------------------------------

using System.Linq.Expressions;

namespace Spirekit.API.EntityFramework.Repositories;

/// <summary>
/// Defines pagination operations for entities.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public interface IPagination<T> where T : class
{
    /// <summary>
    /// Retrieves a paginated list of active entities.
    /// </summary>
    Task<PaginatedResult<T>> ListPagedAsync(int page, int pageSize);

    /// <summary>
    /// Retrieves a paginated list of active entities matching the given filter expression.
    /// </summary>
    Task<PaginatedResult<T>> ListPagedFilteredAsync(
        Expression<Func<T, bool>> filter,
        int page,
        int pageSize);
}
