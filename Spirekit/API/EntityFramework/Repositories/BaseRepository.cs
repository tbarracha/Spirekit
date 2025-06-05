// -----------------------------------------------------------------------------
// Author: Tiago Barracha <ti.barracha@gmail.com>
// Created with AI assistance (ChatGPT)
// Description: Provides a generic base repository implementation with support for 
// pagination, batch operations, and soft deletion using Entity Framework Core.
// -----------------------------------------------------------------------------
//
// USAGE:
//
// 1. Create your concrete repository by inheriting from BaseRepository:
// 
//   public class UserRepository : BaseRepository<User, Guid, AppDbContext>
//   {
//       public UserRepository(AppDbContext context) : base(context) {}
//
//       public override Task<User?> GetByIdAsync(Guid id) =>
//           _dbSet.FirstOrDefaultAsync(u => u.Id == id && u.StateFlag == StateFlags.ACTIVE);
//   }
//
// 2. Register IRepository<T, TId> as a scoped service if needed:
// 
//   services.AddScoped<IRepository<User, Guid>, UserRepository>();
//
// -----------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Spirekit.Core.Constants;
using Spirekit.Core.Interfaces;
using Spirekit.Events;
using System.Linq.Expressions;

namespace Spirekit.API.EntityFramework.Repositories;

/// <summary>
/// Provides a generic base implementation for common data access operations (CRUD) 
/// over a specific entity type using Entity Framework Core.
/// </summary>
/// <typeparam name="T">The entity type. Must implement ICreatedAt, IUpdatedAt, and IStateFlag.</typeparam>
/// <typeparam name="TId">The primary key type of the entity.</typeparam>
/// <typeparam name="TContext">The EF Core DbContext type for the current repository scope.</typeparam>
public abstract class BaseRepository<T, TId, TContext> : IRepository<T, TId>
    where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    where TContext : DbContext
{
    protected readonly TContext _context;
    protected readonly DbSet<T> _dbSet;

    public LazyEventEmitter<T> OnAfterAdd { get; } = new();
    public LazyEventEmitter<List<T>> OnAfterAddRange { get; } = new();
    public LazyEventEmitter<T> OnAfterUpdate { get; } = new();
    public LazyEventEmitter<List<T>> OnAfterUpdateRange { get; } = new();
    public LazyEventEmitter<T> OnAfterDelete { get; } = new();
    public LazyEventEmitter<List<T>> OnAfterDeleteRange { get; } = new();

    protected BaseRepository(TContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // --- Read ---

    public abstract Task<T?> GetByIdAsync(TId id);

    public virtual async Task<IReadOnlyList<T>> ListAsync()
        => await _dbSet.Where(e => e.StateFlag == StateFlags.ACTIVE).ToListAsync();

    public virtual async Task<PaginatedResult<T>> ListPagedAsync(int page, int pageSize)
    {
        var query = _dbSet.Where(e => e.StateFlag == StateFlags.ACTIVE);
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<T>(items, totalCount, page, pageSize);
    }

    public virtual async Task<PaginatedResult<T>> ListPagedFilteredAsync(
        Expression<Func<T, bool>> filter,
        int page,
        int pageSize)
    {
        var query = _dbSet
            .Where(e => e.StateFlag == StateFlags.ACTIVE)
            .Where(filter);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<T>(items, totalCount, page, pageSize);
    }

    // --- Create ---

    public virtual async Task<T> AddAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

        OnAfterAdd.Emit(entity);
        return entity;
    }

    public virtual async Task<IReadOnlyList<T>> AddRangeAsync(IEnumerable<T> entities)
    {
        var entityList = entities.ToList();
        var utcNow = DateTime.UtcNow;

        foreach (var entity in entityList)
        {
            entity.CreatedAt = utcNow;
            entity.UpdatedAt = utcNow;
        }

        await _dbSet.AddRangeAsync(entityList);
        await _context.SaveChangesAsync();

        OnAfterAddRange.Emit(entityList);

        return entityList;
    }

    // --- Update ---

    public virtual async Task<T> UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();

        OnAfterUpdate.Emit(entity);
        return entity;
    }

    public virtual async Task<IReadOnlyList<T>> UpdateRangeAsync(IEnumerable<T> entities)
    {
        var entityList = entities.ToList();
        var utcNow = DateTime.UtcNow;

        foreach (var entity in entityList)
            entity.UpdatedAt = utcNow;

        _dbSet.UpdateRange(entityList);
        await _context.SaveChangesAsync();

        OnAfterUpdateRange.Emit(entityList);

        return entityList;
    }

    // --- Delete ---

    public virtual async Task<T> DeleteAsync(T entity)
    {
        entity.StateFlag = StateFlags.DELETED;
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();

        OnAfterDelete.Emit(entity);
        return entity;
    }

    public virtual async Task<IReadOnlyList<T>> DeleteRangeAsync(IEnumerable<T> entities)
    {
        var entityList = entities.ToList();
        var utcNow = DateTime.UtcNow;

        foreach (var entity in entityList)
        {
            entity.StateFlag = StateFlags.DELETED;
            entity.UpdatedAt = utcNow;
        }

        _dbSet.UpdateRange(entityList);
        await _context.SaveChangesAsync();

        OnAfterDeleteRange.Emit(entityList);

        return entityList;
    }

    public virtual async Task<T?> SoftDeleteAsync(TId id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is null) return null;

        entity.StateFlag = StateFlags.DELETED;
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();

        OnAfterDelete.Emit(entity);
        return entity;
    }
}
