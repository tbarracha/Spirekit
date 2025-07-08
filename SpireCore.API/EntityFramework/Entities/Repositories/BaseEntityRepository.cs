using Microsoft.EntityFrameworkCore;
using SpireCore.API.EntityFramework.Entities.Base;
using SpireCore.Constants;
using SpireCore.Lists.Pagination;
using System.Linq.Expressions;

namespace SpireCore.API.EntityFramework.Entities.Repositories;

/// <summary>
/// Base implementation of <see cref="IEntityRepository{T,TId}"/> backed by Entity Framework Core.
/// It centralises the common CRUD logic so concrete repositories only need to inject a
/// <typeparamref name="TContext"/> (a <see cref="DbContext"/>) and, if required, override individual members.
/// </summary>
/// <typeparam name="T">Entity type handled by this repository.</typeparam>
/// <typeparam name="TId">Primary‑key type.</typeparam>
/// <typeparam name="TContext">EF Core <see cref="DbContext"/> used for persistence.</typeparam>
public abstract class BaseEntityRepository<T, TId, TContext> : IEntityRepository<T, TId>, IPagination<T>
    where T : class, IEntity<TId>
    where TContext : DbContext
{
    /// <summary>Underlying EF Core context.</summary>
    protected readonly TContext _context;

    /// <summary>EF Core <see cref="DbSet{TEntity}"/> for <typeparamref name="T"/>.</summary>
    protected readonly DbSet<T> _dbSet;

    public IQueryable<T> Query() => _dbSet.AsQueryable();

    /// <summary>
    /// Creates a new repository bound to <paramref name="context"/>.
    /// </summary>
    protected BaseEntityRepository(TContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // --- Read ---

    public virtual async Task<T?> GetByIdAsync(TId id, string? state = StateFlags.ACTIVE)
    {
        var query = _dbSet.AsQueryable();
        if (state != null)
            query = query.Where(e => e.StateFlag == state);
        return await query.FirstOrDefaultAsync(e => e.Id.Equals(id));
    }

    public virtual async Task<T?> GetFilteredAsync(Expression<Func<T, bool>> predicate, string? state = StateFlags.ACTIVE)
    {
        var query = _dbSet.AsQueryable();
        if (state != null)
            query = query.Where(e => e.StateFlag == state);
        return await query.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<IReadOnlyList<T>> ListFilteredAsync(Expression<Func<T, bool>> filter, string? state = StateFlags.ACTIVE)
    {
        var query = _dbSet.AsQueryable();
        if (state != null)
            query = query.Where(e => e.StateFlag == state);
        return await query.Where(filter).ToListAsync();
    }

    public virtual async Task<IReadOnlyList<T>> ListAsync(string? state = StateFlags.ACTIVE)
    {
        var query = _dbSet.AsQueryable();
        if (state != null)
            query = query.Where(e => e.StateFlag == state);
        return await query.ToListAsync();
    }

    public virtual async Task<PaginatedResult<T>> GetPaginatedResultAsync(int page, int pageSize, string? state = StateFlags.ACTIVE)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dbSet.AsQueryable();
        if (state != null)
            query = query.Where(e => e.StateFlag == state);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<T>(items, totalCount, page, pageSize);
    }

    public virtual async Task<PaginatedResult<T>> GetFilteredPaginatedResultAsync(
        Expression<Func<T, bool>> filter,
        int page,
        int pageSize,
        string? state = StateFlags.ACTIVE)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dbSet.AsQueryable();
        if (state != null)
            query = query.Where(e => e.StateFlag == state);
        query = query.Where(filter);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<T>(items, totalCount, page, pageSize);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        string? state = StateFlags.ACTIVE)
    {
        var query = _dbSet.AsQueryable();

        if (state != null)
            query = query.Where(e => e.StateFlag == state);

        return await query.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        string? state = StateFlags.ACTIVE)
    {
        var query = _dbSet.AsQueryable();
        if (state != null)
            query = query.Where(e => e.StateFlag == state);

        return await query.AnyAsync(predicate);
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<T, bool>> predicate,
        string? state = StateFlags.ACTIVE)
    {
        var query = _dbSet.AsQueryable();
        if (state != null)
            query = query.Where(e => e.StateFlag == state);

        return await query.CountAsync(predicate);
    }

    // --- Create ---

    public virtual async Task<T> AddAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

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

        return entityList;
    }

    // --- Update ---

    public virtual async Task<T> UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();

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

        return entityList;
    }

    // --- Delete ---

    public virtual async Task<T> DeleteAsync(T entity)
    {
        entity.StateFlag = StateFlags.DELETED;
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();

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

        return entity;
    }

    public virtual async Task<T?> RestoreAsync(TId id)
    {
        var entity = await GetByIdAsync(id, StateFlags.DELETED);
        if (entity is null) return null;

        entity.StateFlag = StateFlags.ACTIVE;
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();

        return entity;
    }
}
