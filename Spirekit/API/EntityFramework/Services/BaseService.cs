using Microsoft.EntityFrameworkCore;
using Spirekit.API.EntityFramework.Pagination;
using Spirekit.Core.Constants;
using Spirekit.Core.Interfaces;
using System.Linq.Expressions;

namespace Spirekit.API.EntityFramework.Services;

/// <summary>
/// Provides utility methods and shared logic for domain/application services.
/// Compose one or more repositories per service as needed.
/// </summary>
public abstract class BaseService
{
    protected readonly DbContext _context;

    protected BaseService(DbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Utility for handling transactions across multiple repositories.
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

    // --- Query and CRUD Methods ---

    public virtual async Task<T?> GetByIdAsync<T, TId>(TId id)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        var dbSet = _context.Set<T>();
        return await dbSet.FirstOrDefaultAsync(e => EF.Property<TId>(e, "Id").Equals(id) && e.StateFlag == StateFlags.ACTIVE);
    }

    public virtual async Task<IReadOnlyList<T>> ListAsync<T>()
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        return await _context.Set<T>()
            .Where(e => e.StateFlag == StateFlags.ACTIVE)
            .ToListAsync();
    }

    public virtual async Task<PaginatedResult<T>> ListPagedAsync<T>(int page, int pageSize)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var dbSet = _context.Set<T>();
        var query = dbSet.Where(e => e.StateFlag == StateFlags.ACTIVE);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<T>(items, totalCount, page, pageSize);
    }

    public virtual async Task<PaginatedResult<T>> ListPagedFilteredAsync<T>(
        Expression<Func<T, bool>> filter,
        int page,
        int pageSize)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var dbSet = _context.Set<T>();
        var query = dbSet
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

    public virtual async Task<T> AddAsync<T>(T entity)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public virtual async Task<IReadOnlyList<T>> AddRangeAsync<T>(IEnumerable<T> entities)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
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
    }

    // --- Update ---

    public virtual async Task<T> UpdateAsync<T>(T entity)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public virtual async Task<IReadOnlyList<T>> UpdateRangeAsync<T>(IEnumerable<T> entities)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        var entityList = entities.ToList();
        var utcNow = DateTime.UtcNow;

        foreach (var entity in entityList)
            entity.UpdatedAt = utcNow;

        _context.Set<T>().UpdateRange(entityList);
        await _context.SaveChangesAsync();

        return entityList;
    }

    // --- Delete (Soft Delete) ---

    public virtual async Task<T> DeleteAsync<T>(T entity)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        entity.StateFlag = StateFlags.DELETED;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public virtual async Task<IReadOnlyList<T>> DeleteRangeAsync<T>(IEnumerable<T> entities)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
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
    }

    public virtual async Task<T?> SoftDeleteAsync<T, TId>(TId id)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        var dbSet = _context.Set<T>();
        var entity = await dbSet.FirstOrDefaultAsync(e => EF.Property<TId>(e, "Id").Equals(id));
        if (entity is null) return null;

        entity.StateFlag = StateFlags.DELETED;
        entity.UpdatedAt = DateTime.UtcNow;
        dbSet.Update(entity);
        await _context.SaveChangesAsync();

        return entity;
    }
}
