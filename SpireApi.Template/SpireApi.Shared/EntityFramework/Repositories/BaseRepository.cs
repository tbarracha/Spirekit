using Microsoft.EntityFrameworkCore;
using SpireCore.Constants;
using SpireCore.Lists.Pagination;
using System.Linq.Expressions;

namespace SpireApi.Shared.EntityFramework.Repositories;

/// <summary>
/// Provides a generic base implementation for common data access operations (CRUD)
/// over a specific entity type using Entity Framework Core.
/// </summary>
/// <typeparam name="T">
/// The entity type for this repository (must implement <see cref="IRepoEntity{TId}"/>).
/// </typeparam>
/// <typeparam name="TId">
/// The type of the primary key for the entity (e.g., <c>Guid</c>, <c>int</c>).
/// </typeparam>
/// <typeparam name="TContext">
/// The DbContext type this repository will use (must inherit from <see cref="DbContext"/>).
/// </typeparam>
public abstract class BaseRepository<T, TId, TContext> : IRepository<T, TId>, IPagination<T>
    where T : class, IRepoEntity<TId>
    where TContext : DbContext
{
    protected readonly TContext _context;
    protected readonly DbSet<T> _dbSet;

    public IQueryable<T> Query() => _dbSet.AsQueryable();

    protected BaseRepository(TContext context)
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

    public virtual async Task<PaginatedResult<T>> ListPagedAsync(int page, int pageSize, string? state = StateFlags.ACTIVE)
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

    public virtual async Task<PaginatedResult<T>> ListPagedFilteredAsync(
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
