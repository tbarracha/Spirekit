using Microsoft.EntityFrameworkCore;
using SpireCore.Constants;

namespace SpireApi.Application.Shared.Entities;

public class GuidEntityByRepository<T> : GuidEntityRepository<T>, IGuidEntityByRepository<T>
    where T : GuidEntityBy
{
    public GuidEntityByRepository(GuidEntityDbContext context) : base(context) { }

    public async Task<IReadOnlyList<T>> ListCreatedByAsync(string createdBy, string? state = StateFlags.ACTIVE)
    {
        var query = _dbSet.AsQueryable();
        if (state != null)
            query = query.Where(e => e.StateFlag == state);
        return await query.Where(e => e.CreatedBy == createdBy).ToListAsync();
    }

    public async Task<IReadOnlyList<T>> ListUpdatedByAsync(string updatedBy, string? state = StateFlags.ACTIVE)
    {
        var query = _dbSet.AsQueryable();
        if (state != null)
            query = query.Where(e => e.StateFlag == state);
        return await query.Where(e => e.UpdatedBy == updatedBy).ToListAsync();
    }

    public async Task<T?> GetCreatedByAsync(string createdBy, string? state = StateFlags.ACTIVE)
    {
        var query = _dbSet.AsQueryable();
        if (state != null)
            query = query.Where(e => e.StateFlag == state);
        return await query.FirstOrDefaultAsync(e => e.CreatedBy == createdBy);
    }

    public async Task<T?> GetUpdatedByAsync(string updatedBy, string? state = StateFlags.ACTIVE)
    {
        var query = _dbSet.AsQueryable();
        if (state != null)
            query = query.Where(e => e.StateFlag == state);
        return await query.FirstOrDefaultAsync(e => e.UpdatedBy == updatedBy);
    }
}
