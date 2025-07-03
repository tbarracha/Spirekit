// -----------------------------------------------------------------------------
// Author: Tiago Barracha <ti.barracha@gmail.com>
// Created with AI assistance (ChatGPT)
//
// Description: Provides a generic base service for Entity Framework with support
// for multiple DbContexts, transactional operations, CRUD, pagination, filtering,
// and soft deletion/restoration. Dynamically resolves the correct context for
// each entity type and caches this mapping for fast runtime performance.
// Designed for use in domain/application services spanning multiple databases.
// -----------------------------------------------------------------------------
//
// USAGE:
//
// 1. Create your domain/application service by inheriting from BaseMultiContextService:
//
//    public class AppUserAccountService : BaseMultiContextService
//    {
//        public AppUserAccountService(List<DbContext> contexts) : base(contexts) {}
//        // Add your domain logic, reuse generic methods, etc.
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
//    You can manage entities from multiple contexts with a single service instance.
//
// 4. Register your service with Dependency Injection in Program.cs (or Startup.cs):
//
//    services.AddScoped<AppUserAccountService>(sp =>
//        new AppUserAccountService(new List<DbContext>
//        {
//            sp.GetRequiredService<AppDbContext>(),
//            sp.GetRequiredService<AuditDbContext>(),
//            // ...add other contexts as needed
//        })
//    );
//
// NOTE:
// - Each entity type can be managed by only one context. The mapping is built at construction
//   and throws if any entity appears in multiple contexts.
// - Transactions and CRUD operations are always routed to the correct context for the entity.
//
// -----------------------------------------------------------------------------


using Microsoft.EntityFrameworkCore;
using SpireCore.Abstractions.Interfaces;
using SpireCore.Constants;
using SpireCore.Lists.Pagination;
using System.Linq.Expressions;

/// <summary>
/// Provides a base service for managing multiple EF Core DbContexts,
/// with fast entity-to-context resolution, generic CRUD, filtering, pagination,
/// and per-context transactional support.
/// </summary>
public abstract class EfBaseMultiContextService
{
    private readonly Dictionary<Type, DbContext> _entityTypeToContext;

    /// <summary>
    /// Initializes the multi-context service and builds a fast type-to-context lookup.
    /// Throws if any entity type is registered in more than one context.
    /// </summary>
    /// <param name="contexts">The list of managed DbContexts.</param>
    protected EfBaseMultiContextService(List<DbContext> contexts)
    {
        _entityTypeToContext = new Dictionary<Type, DbContext>();

        foreach (var context in contexts)
        {
            var entityTypes = context.Model.GetEntityTypes().Select(t => t.ClrType);
            foreach (var type in entityTypes)
            {
                if (_entityTypeToContext.ContainsKey(type))
                    throw new InvalidOperationException(
                        $"Type {type.Name} is already registered with another DbContext.");

                _entityTypeToContext[type] = context;
            }
        }
    }

    /// <summary>
    /// Gets the DbContext responsible for managing the specified entity type.
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    protected DbContext GetContextForType<T>() where T : class
    {
        if (_entityTypeToContext.TryGetValue(typeof(T), out var context))
            return context;
        throw new InvalidOperationException(
            $"No DbContext found for entity type {typeof(T).Name}");
    }

    /// <summary>
    /// Runs a function within a transaction on the context that manages entity type T.
    /// Commits on success, rolls back on error.
    /// </summary>
    /// <typeparam name="T">Entity type used to resolve the DbContext</typeparam>
    /// <typeparam name="TResult">Result type</typeparam>
    /// <param name="action">Action to execute, receives the resolved DbContext</param>
    protected async Task<TResult> InTransactionAsync<T, TResult>(Func<DbContext, Task<TResult>> action)
        where T : class
    {
        var context = GetContextForType<T>();
        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var result = await action(context);
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
    public virtual async Task<IReadOnlyList<T>> GetFilteredAsync<T>(
        Expression<Func<T, bool>> filter,
        string state = StateFlags.ACTIVE)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        var ctx = GetContextForType<T>();
        return await ctx.Set<T>()
            .Where(e => e.StateFlag == state)
            .Where(filter)
            .ToListAsync();
    }

    /// <summary>
    /// Returns an entity by id and state.
    /// </summary>
    public virtual async Task<T?> GetByIdAsync<T, TId>(TId id, string state = StateFlags.ACTIVE)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        var ctx = GetContextForType<T>();
        return await ctx.Set<T>()
            .FirstOrDefaultAsync(e => EF.Property<TId>(e, "Id").Equals(id) && e.StateFlag == state);
    }

    /// <summary>
    /// Lists all entities of type T with the given state.
    /// </summary>
    public virtual async Task<IReadOnlyList<T>> ListAsync<T>(string state = StateFlags.ACTIVE)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        var ctx = GetContextForType<T>();
        return await ctx.Set<T>()
            .Where(e => e.StateFlag == state)
            .ToListAsync();
    }

    /// <summary>
    /// Returns a paged list of entities of type T for the given state.
    /// </summary>
    public virtual async Task<PaginatedResult<T>> ListPagedAsync<T>(int page, int pageSize, string state = StateFlags.ACTIVE)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var ctx = GetContextForType<T>();
        var query = ctx.Set<T>().Where(e => e.StateFlag == state);

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
    public virtual async Task<PaginatedResult<T>> ListPagedFilteredAsync<T>(
        Expression<Func<T, bool>> filter,
        int page,
        int pageSize,
        string state = StateFlags.ACTIVE)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var ctx = GetContextForType<T>();
        var query = ctx.Set<T>().Where(e => e.StateFlag == state).Where(filter);

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
        return await InTransactionAsync<T, T>(async ctx =>
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await ctx.Set<T>().AddAsync(entity);
            await ctx.SaveChangesAsync();
            return entity;
        });
    }

    /// <summary>
    /// Adds multiple entities of type T and returns them. Runs in a transaction.
    /// </summary>
    public virtual async Task<IReadOnlyList<T>> AddRangeAsync<T>(IEnumerable<T> entities)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        return await InTransactionAsync<T, IReadOnlyList<T>>(async ctx =>
        {
            var entityList = entities.ToList();
            var utcNow = DateTime.UtcNow;

            foreach (var entity in entityList)
            {
                entity.CreatedAt = utcNow;
                entity.UpdatedAt = utcNow;
            }

            await ctx.Set<T>().AddRangeAsync(entityList);
            await ctx.SaveChangesAsync();

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
        return await InTransactionAsync<T, T>(async ctx =>
        {
            entity.UpdatedAt = DateTime.UtcNow;
            ctx.Set<T>().Update(entity);
            await ctx.SaveChangesAsync();
            return entity;
        });
    }

    /// <summary>
    /// Updates multiple entities of type T and returns them. Runs in a transaction.
    /// </summary>
    public virtual async Task<IReadOnlyList<T>> UpdateRangeAsync<T>(IEnumerable<T> entities)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        return await InTransactionAsync<T, IReadOnlyList<T>>(async ctx =>
        {
            var entityList = entities.ToList();
            var utcNow = DateTime.UtcNow;

            foreach (var entity in entityList)
                entity.UpdatedAt = utcNow;

            ctx.Set<T>().UpdateRange(entityList);
            await ctx.SaveChangesAsync();

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
        return await InTransactionAsync<T, T>(async ctx =>
        {
            entity.StateFlag = StateFlags.DELETED;
            entity.UpdatedAt = DateTime.UtcNow;
            ctx.Set<T>().Update(entity);
            await ctx.SaveChangesAsync();
            return entity;
        });
    }

    /// <summary>
    /// Marks multiple entities as deleted (soft delete) and returns them. Runs in a transaction.
    /// </summary>
    public virtual async Task<IReadOnlyList<T>> DeleteRangeAsync<T>(IEnumerable<T> entities)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        return await InTransactionAsync<T, IReadOnlyList<T>>(async ctx =>
        {
            var entityList = entities.ToList();
            var utcNow = DateTime.UtcNow;

            foreach (var entity in entityList)
            {
                entity.StateFlag = StateFlags.DELETED;
                entity.UpdatedAt = utcNow;
            }

            ctx.Set<T>().UpdateRange(entityList);
            await ctx.SaveChangesAsync();

            return entityList;
        });
    }

    /// <summary>
    /// Finds an entity by id, marks it as deleted (soft delete), and returns it. Runs in a transaction.
    /// </summary>
    public virtual async Task<T?> SoftDeleteAsync<T, TId>(TId id)
        where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        return await InTransactionAsync<T, T?>(async ctx =>
        {
            var dbSet = ctx.Set<T>();
            var entity = await dbSet.FirstOrDefaultAsync(e => EF.Property<TId>(e, "Id").Equals(id));
            if (entity is null) return null;

            entity.StateFlag = StateFlags.DELETED;
            entity.UpdatedAt = DateTime.UtcNow;
            dbSet.Update(entity);
            await ctx.SaveChangesAsync();

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
        return await InTransactionAsync<T, T?>(async ctx =>
        {
            var dbSet = ctx.Set<T>();
            var entity = await dbSet.FirstOrDefaultAsync(e => EF.Property<TId>(e, "Id").Equals(id));
            if (entity is null) return null;

            entity.StateFlag = restoreState;
            entity.UpdatedAt = DateTime.UtcNow;
            dbSet.Update(entity);
            await ctx.SaveChangesAsync();

            return entity;
        });
    }
}
