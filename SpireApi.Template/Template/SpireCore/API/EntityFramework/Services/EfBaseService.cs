// -----------------------------------------------------------------------------
// Author: Tiago Barracha <ti.barracha@gmail.com>
// Created with AI assistance (ChatGPT)
//
// Description: Provides a generic base service for Entity Framework with 
// transactional support, CRUD operations, pagination, filtering, and soft
// deletion/restoration. Designed for use in domain/application services.
// -----------------------------------------------------------------------------
//
// USAGE:
//
// 1. Create your domain/application service by inheriting from BaseService:
// 
//    public class AppUserAccountService : BaseService
//    {
//        public AppUserAccountService(AppDbContext context) : base(context) {}
//        // Add your domain logic, reuse BaseService methods, etc.
//    }
//
// 2. Use the provided generic methods for common entity operations:
// 
//    var user = await service.GetByIdAsync<User, Guid>(userId);
//    var page = await service.ListPagedAsync<User>(1, 20);
//    var filtered = await service.GetFilteredAsync<User>(u => u.Email == email);
//    await service.SoftDeleteAsync<User, Guid>(userId);
//    await service.SoftRestoreAsync<User, Guid>(userId);
//
// 3. Compose or extend additional repositories/services as needed in your own class.
//
// -----------------------------------------------------------------------------



using Microsoft.EntityFrameworkCore;
using SpireCore.Abstractions.Interfaces;
using SpireCore.Constants;
using SpireCore.Lists.Pagination;
using System.Linq.Expressions;

namespace SpireCore.API.EntityFramework.Services;

/// <summary>
/// Provides generic CRUD, filtering, and soft-delete/restore operations with transactional support for EF entities.
/// </summary>
public abstract class EfBaseService
{
    protected readonly DbContext _context;

    /// <summary>
    /// Constructs the base service with the supplied DbContext.
    /// </summary>
    protected EfBaseService(DbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Runs a function within a transaction, committing on success or rolling back on error.
    /// </summary>
    protected async Task<TResult> InTransactionAsync<TResult>(Func<Task<TResult>> action)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var result = await action();
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // --- Query Methods ---

    /// <summary>
    /// Returns all entities of type T matching the filter and state.
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="filter">Predicate for filtering entities</param>
    /// <param name="state">Desired entity state (default: ACTIVE)</param>
    public virtual async Task<IReadOnlyList<T>> GetFilteredAsync<T>(
        Expression<Func<T, bool>> filter,
        string state = StateFlags.ACTIVE)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        var dbSet = _context.Set<T>();
        return await dbSet
            .Where(e => e.StateFlag == state)
            .Where(filter)
            .ToListAsync();
    }

    /// <summary>
    /// Returns an entity by id and state.
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="TId">Entity key type</typeparam>
    /// <param name="id">Entity id</param>
    /// <param name="state">Desired entity state (default: ACTIVE)</param>
    public virtual async Task<T?> GetByIdAsync<T, TId>(TId id, string state = StateFlags.ACTIVE)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        var dbSet = _context.Set<T>();
        return await dbSet.FirstOrDefaultAsync(e => EF.Property<TId>(e, "Id").Equals(id) && e.StateFlag == state);
    }

    /// <summary>
    /// Lists all entities of type T with the given state.
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="state">Desired entity state (default: ACTIVE)</param>
    public virtual async Task<IReadOnlyList<T>> ListAsync<T>(string state = StateFlags.ACTIVE)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        return await _context.Set<T>()
            .Where(e => e.StateFlag == state)
            .ToListAsync();
    }

    /// <summary>
    /// Returns a paged list of entities of type T for the given state.
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="page">1-based page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="state">Desired entity state (default: ACTIVE)</param>
    public virtual async Task<PaginatedResult<T>> ListPagedAsync<T>(int page, int pageSize, string state = StateFlags.ACTIVE)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var dbSet = _context.Set<T>();
        var query = dbSet.Where(e => e.StateFlag == state);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<T>(items, totalCount, page, pageSize);
    }

    /// <summary>
    /// Returns a paged list of entities of type T filtered by predicate and state.
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="filter">Predicate for filtering entities</param>
    /// <param name="page">1-based page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="state">Desired entity state (default: ACTIVE)</param>
    public virtual async Task<PaginatedResult<T>> ListPagedFilteredAsync<T>(
        Expression<Func<T, bool>> filter,
        int page,
        int pageSize,
        string state = StateFlags.ACTIVE)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var dbSet = _context.Set<T>();
        var query = dbSet.Where(e => e.StateFlag == state).Where(filter);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<T>(items, totalCount, page, pageSize);
    }

    // --- Create ---

    /// <summary>
    /// Adds a new entity of type T and returns it. Runs in a transaction.
    /// </summary>
    public virtual async Task<T> AddAsync<T>(T entity)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        return await InTransactionAsync(async () =>
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        });
    }

    /// <summary>
    /// Adds multiple entities of type T and returns them. Runs in a transaction.
    /// </summary>
    public virtual async Task<IReadOnlyList<T>> AddRangeAsync<T>(IEnumerable<T> entities)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        return await InTransactionAsync(async () =>
        {
            var entityList = entities.ToList();
            var utcNow = DateTime.UtcNow;

            foreach (var entity in entityList)
            {
                entity.CreatedAt = utcNow;
                entity.UpdatedAt = utcNow;
            }

            await _context.Set<T>().AddRangeAsync(entityList);
            await _context.SaveChangesAsync();

            return entityList;
        });
    }

    // --- Update ---

    /// <summary>
    /// Updates an entity of type T and returns it. Runs in a transaction.
    /// </summary>
    public virtual async Task<T> UpdateAsync<T>(T entity)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        return await InTransactionAsync(async () =>
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        });
    }

    /// <summary>
    /// Updates multiple entities of type T and returns them. Runs in a transaction.
    /// </summary>
    public virtual async Task<IReadOnlyList<T>> UpdateRangeAsync<T>(IEnumerable<T> entities)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        return await InTransactionAsync(async () =>
        {
            var entityList = entities.ToList();
            var utcNow = DateTime.UtcNow;

            foreach (var entity in entityList)
                entity.UpdatedAt = utcNow;

            _context.Set<T>().UpdateRange(entityList);
            await _context.SaveChangesAsync();

            return entityList;
        });
    }

    // --- Delete (Soft Delete) ---

    /// <summary>
    /// Marks an entity as deleted (soft delete) and returns it. Runs in a transaction.
    /// </summary>
    public virtual async Task<T> DeleteAsync<T>(T entity)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        return await InTransactionAsync(async () =>
        {
            entity.StateFlag = StateFlags.DELETED;
            entity.UpdatedAt = DateTime.UtcNow;
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        });
    }

    /// <summary>
    /// Marks multiple entities as deleted (soft delete) and returns them. Runs in a transaction.
    /// </summary>
    public virtual async Task<IReadOnlyList<T>> DeleteRangeAsync<T>(IEnumerable<T> entities)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        return await InTransactionAsync(async () =>
        {
            var entityList = entities.ToList();
            var utcNow = DateTime.UtcNow;

            foreach (var entity in entityList)
            {
                entity.StateFlag = StateFlags.DELETED;
                entity.UpdatedAt = utcNow;
            }

            _context.Set<T>().UpdateRange(entityList);
            await _context.SaveChangesAsync();

            return entityList;
        });
    }

    /// <summary>
    /// Finds an entity by id, marks it as deleted (soft delete), and returns it. Runs in a transaction.
    /// </summary>
    public virtual async Task<T?> SoftDeleteAsync<T, TId>(TId id)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        return await InTransactionAsync(async () =>
        {
            var dbSet = _context.Set<T>();
            var entity = await dbSet.FirstOrDefaultAsync(e => EF.Property<TId>(e, "Id").Equals(id));
            if (entity is null) return null;

            entity.StateFlag = StateFlags.DELETED;
            entity.UpdatedAt = DateTime.UtcNow;
            dbSet.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        });
    }

    // --- Restore (Soft Restore) ---

    /// <summary>
    /// Finds a deleted entity by id, restores it by setting its state (default: ACTIVE), and returns it. Runs in a transaction.
    /// </summary>
    public virtual async Task<T?> SoftRestoreAsync<T, TId>(TId id, string restoreState = StateFlags.ACTIVE)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        return await InTransactionAsync(async () =>
        {
            var dbSet = _context.Set<T>();
            var entity = await dbSet.FirstOrDefaultAsync(e => EF.Property<TId>(e, "Id").Equals(id));
            if (entity is null) return null;

            entity.StateFlag = restoreState;
            entity.UpdatedAt = DateTime.UtcNow;
            dbSet.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        });
    }
}

