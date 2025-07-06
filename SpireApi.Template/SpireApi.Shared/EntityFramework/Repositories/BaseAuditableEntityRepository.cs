using Microsoft.EntityFrameworkCore;
using SpireApi.Shared.EntityFramework.Entities.Base;
using SpireCore.Constants;
using SpireCore.Lists.Pagination;
using System.Linq.Expressions;

namespace SpireApi.Shared.EntityFramework.Repositories;

public abstract class BaseAuditableEntityRepository<T, TId, TContext> : BaseEntityRepository<T, TId, TContext>, IAuditableEntityRepository<T, TId>
    where T : class, IAuditableEntity<TId>
    where TContext : DbContext
{
    protected BaseAuditableEntityRepository(TContext context) : base(context)
    {
    }

    // --- Read ---

    public virtual async Task<T?> GetByIdAsync(TId id, string actor, string? state = StateFlags.ACTIVE)
    {
        var query = _dbSet.AsQueryable();
        
        if (state != null)
            query = query.Where(e => e.StateFlag == state);
        
        query = query.Where(e => e.CreatedBy == actor);

        return await query.FirstOrDefaultAsync(e => e.Id.Equals(id));
    }

    public virtual async Task<T?> GetFilteredAsync(Expression<Func<T, bool>> predicate, string actor, string? state = StateFlags.ACTIVE)
    {
        var query = _dbSet.AsQueryable();

        if (state != null)
            query = query.Where(e => e.StateFlag == state);

        query = query.Where(e => e.CreatedBy == actor);

        return await query.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<PaginatedResult<T>> GetFilteredPaginatedResultAsync(
        Expression<Func<T, bool>> filter,
        string actor,
        int page,
        int pageSize,
        string? state = StateFlags.ACTIVE)
    {
        var query = _dbSet.AsQueryable();

        if (state != null)
            query = query.Where(e => e.StateFlag == state);

        query = query.Where(e => e.CreatedBy == actor && filter.Compile().Invoke(e));

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<T>(items, totalCount, page, pageSize);
    }

    public virtual async Task<PaginatedResult<T>> GetPaginatedResultAsync(
        string actor,
        int page,
        int pageSize,
        string? state = StateFlags.ACTIVE)
    {
        var query = _dbSet.AsQueryable();

        if (state != null)
            query = query.Where(e => e.StateFlag == state);

        query = query.Where(e => e.CreatedBy == actor);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<T>(items, totalCount, page, pageSize);
    }


    // --- Create ---

    public virtual async Task<T> AddAsync(T entity, string actor)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.CreatedBy = actor;
        entity.UpdatedBy = actor;

        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public virtual async Task<IReadOnlyList<T>> AddRangeAsync(IEnumerable<T> entities, string actor)
    {
        var entityList = entities.ToList();
        var utcNow = DateTime.UtcNow;

        foreach (var entity in entityList)
        {
            entity.CreatedAt = utcNow;
            entity.UpdatedAt = utcNow;
            entity.CreatedBy = actor;
            entity.UpdatedBy = actor;
        }

        await _dbSet.AddRangeAsync(entityList);
        await _context.SaveChangesAsync();

        return entityList;
    }

    // --- Update ---

    public virtual async Task<T> UpdateAsync(T entity, string actor)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = actor;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public virtual async Task<IReadOnlyList<T>> UpdateRangeAsync(IEnumerable<T> entities, string actor)
    {
        var entityList = entities.ToList();
        var utcNow = DateTime.UtcNow;

        foreach (var entity in entityList)
        {
            entity.UpdatedAt = utcNow;
            entity.UpdatedBy = actor;
        }

        _dbSet.UpdateRange(entityList);
        await _context.SaveChangesAsync();

        return entityList;
    }

    // --- Delete ---

    public virtual async Task<T> DeleteAsync(T entity, string actor)
    {
        entity.StateFlag = StateFlags.DELETED;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = actor;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public virtual async Task<IReadOnlyList<T>> DeleteRangeAsync(IEnumerable<T> entities, string actor)
    {
        var entityList = entities.ToList();
        var utcNow = DateTime.UtcNow;

        foreach (var entity in entityList)
        {
            entity.StateFlag = StateFlags.DELETED;
            entity.UpdatedAt = utcNow;
            entity.UpdatedBy = actor;
        }

        _dbSet.UpdateRange(entityList);
        await _context.SaveChangesAsync();

        return entityList;
    }

    public virtual async Task<T?> SoftDeleteAsync(TId id, string actor)
    {
        var entity = await GetByIdAsync(id, actor);
        if (entity is null) return null;

        entity.StateFlag = StateFlags.DELETED;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = actor;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public virtual async Task<T?> RestoreAsync(TId id, string actor)
    {
        var entity = await GetByIdAsync(id, actor, StateFlags.DELETED);
        if (entity is null) return null;

        entity.StateFlag = StateFlags.ACTIVE;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = actor;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();

        return entity;
    }
}
