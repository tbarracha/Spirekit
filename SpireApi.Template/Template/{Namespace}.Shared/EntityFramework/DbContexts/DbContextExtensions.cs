// -----------------------------------------------------------------------------
// Author: Tiago Barracha <ti.barracha@gmail.com>
// Created with AI assistance (ChatGPT)
// Description: Provides static, generic extension methods for CRUD, pagination, 
// and soft deletion on any EF Core DbContext and entity type implementing core 
// interfaces. Designed for maximum flexibility and reusability.
// -----------------------------------------------------------------------------
//
// USAGE:
//
// 1. Call extension methods directly on your DbContext instance:
// 
//   var user = await context.GetByIdAsync<User, Guid, AppDbContext>(userId);
//   var page = await context.ListPagedAsync<User, AppDbContext>(1, 10);
//   await context.AddAsync<User, AppDbContext>(newUser);
//
// 2. Methods work for any entity implementing ICreatedAt, IUpdatedAt, IStateFlag.
//    No inheritance required.
//
// -----------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using {Namespace}.Shared.EntityFramework.DbContexts;
using SpireCore.Abstractions.Interfaces;
using SpireCore.Constants;
using SpireCore.Lists.Pagination;
using System.Linq.Expressions;

namespace {Namespace}.Shared.EntityFramework.DbContexts;

public static class DbContextExtensions
{
    // --- Read ---

    public static async Task<T?> GetByIdAsync<T, TId, TContext>(this TContext context, TId id)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
        where TContext : DbContext
    {
        var dbSet = context.Set<T>();
        // Assumes your PK is named "Id"
        return await dbSet.FirstOrDefaultAsync(e => EF.Property<TId>(e, "Id").Equals(id) && e.StateFlag == StateFlags.ACTIVE);
    }

    public static async Task<IReadOnlyList<T>> ListAsync<T, TContext>(this TContext context)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
        where TContext : DbContext
    {
        return await context.Set<T>()
            .Where(e => e.StateFlag == StateFlags.ACTIVE)
            .ToListAsync();
    }

    public static async Task<PaginatedResult<T>> ListPagedAsync<T, TContext>(this TContext context, int page, int pageSize)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
        where TContext : DbContext
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var dbSet = context.Set<T>();
        var query = dbSet.Where(e => e.StateFlag == StateFlags.ACTIVE);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<T>(items, totalCount, page, pageSize);
    }

    public static async Task<PaginatedResult<T>> ListPagedFilteredAsync<T, TContext>(
        this TContext context,
        Expression<Func<T, bool>> filter,
        int page,
        int pageSize)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
        where TContext : DbContext
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var dbSet = context.Set<T>();
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

    public static async Task<T> AddAsync<T, TContext>(this TContext context, T entity)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
        where TContext : DbContext
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();

        return entity;
    }

    public static async Task<IReadOnlyList<T>> AddRangeAsync<T, TContext>(this TContext context, IEnumerable<T> entities)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
        where TContext : DbContext
    {
        var entityList = entities.ToList();
        var utcNow = DateTime.UtcNow;

        foreach (var entity in entityList)
        {
            entity.CreatedAt = utcNow;
            entity.UpdatedAt = utcNow;
        }

        await context.Set<T>().AddRangeAsync(entityList);
        await context.SaveChangesAsync();

        return entityList;
    }

    // --- Update ---

    public static async Task<T> UpdateAsync<T, TContext>(this TContext context, T entity)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
        where TContext : DbContext
    {
        entity.UpdatedAt = DateTime.UtcNow;
        context.Set<T>().Update(entity);
        await context.SaveChangesAsync();

        return entity;
    }

    public static async Task<IReadOnlyList<T>> UpdateRangeAsync<T, TContext>(this TContext context, IEnumerable<T> entities)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
        where TContext : DbContext
    {
        var entityList = entities.ToList();
        var utcNow = DateTime.UtcNow;

        foreach (var entity in entityList)
            entity.UpdatedAt = utcNow;

        context.Set<T>().UpdateRange(entityList);
        await context.SaveChangesAsync();

        return entityList;
    }

    // --- Delete (Soft Delete) ---

    public static async Task<T> DeleteAsync<T, TContext>(this TContext context, T entity)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
        where TContext : DbContext
    {
        entity.StateFlag = StateFlags.DELETED;
        entity.UpdatedAt = DateTime.UtcNow;
        context.Set<T>().Update(entity);
        await context.SaveChangesAsync();

        return entity;
    }

    public static async Task<IReadOnlyList<T>> DeleteRangeAsync<T, TContext>(this TContext context, IEnumerable<T> entities)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
        where TContext : DbContext
    {
        var entityList = entities.ToList();
        var utcNow = DateTime.UtcNow;

        foreach (var entity in entityList)
        {
            entity.StateFlag = StateFlags.DELETED;
            entity.UpdatedAt = utcNow;
        }

        context.Set<T>().UpdateRange(entityList);
        await context.SaveChangesAsync();

        return entityList;
    }

    public static async Task<T?> SoftDeleteAsync<T, TId, TContext>(this TContext context, TId id)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
        where TContext : DbContext
    {
        var dbSet = context.Set<T>();
        var entity = await dbSet.FirstOrDefaultAsync(e => EF.Property<TId>(e, "Id").Equals(id));
        if (entity is null) return null;

        entity.StateFlag = StateFlags.DELETED;
        entity.UpdatedAt = DateTime.UtcNow;
        dbSet.Update(entity);
        await context.SaveChangesAsync();

        return entity;
    }
}


