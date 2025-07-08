﻿using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Authentication.Domain.AuthAudits.Models;
using SpireApi.Application.Modules.Authentication.Infrastructure;
using SpireCore.API.EntityFramework.Entities.Repositories;
using SpireCore.Constants;

namespace SpireApi.Application.Modules.Authentication.Domain.AuthAudits.Repositories;

public class AuthAuditRepository : BaseEntityRepository<AuthAudit, Guid, BaseAuthDbContext>
{
    public AuthAuditRepository(BaseAuthDbContext context) : base(context) { }

    /// <summary>
    /// Persist a new audit record and save immediately.
    /// </summary>
    public async Task RecordAsync(AuthAudit audit)
    {
        await _dbSet.AddAsync(audit);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Return the N most-recent audit events for a user (default 50).
    /// </summary>
    public async Task<IReadOnlyList<AuthAudit>> ListRecentForUserAsync(
        Guid authUserId,
        int limit = 50)
    {
        return await _dbSet.Where(a =>
                                   a.AuthUserId == authUserId &&
                                   a.StateFlag == StateFlags.ACTIVE)
                           .OrderByDescending(a => a.CreatedAt)
                           .Take(limit)
                           .ToListAsync();
    }

    /// <summary>
    /// Return the N most-recent audit events of a specific type for a user.
    /// </summary>
    public async Task<IReadOnlyList<AuthAudit>> ListByTypeAsync(
        Guid authUserId,
        string type,
        int limit = 50)
    {
        return await _dbSet.Where(a =>
                                   a.AuthUserId == authUserId &&
                                   a.Type == type &&
                                   a.StateFlag == StateFlags.ACTIVE)
                           .OrderByDescending(a => a.CreatedAt)
                           .Take(limit)
                           .ToListAsync();
    }

    /// <summary>
    /// Get the latest audit entry (of any type) for a user.
    /// </summary>
    public async Task<AuthAudit?> GetLatestAsync(Guid authUserId)
    {
        return await _dbSet.Where(a =>
                                   a.AuthUserId == authUserId &&
                                   a.StateFlag == StateFlags.ACTIVE)
                           .OrderByDescending(a => a.CreatedAt)
                           .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Count successful login attempts since the given point in time.
    /// </summary>
    public async Task<int> CountSuccessfulLoginsSinceAsync(Guid authUserId, DateTime sinceUtc)
    {
        return await _dbSet.CountAsync(a =>
            a.AuthUserId == authUserId &&
            a.Type == AuthAuditType.Login &&
            a.WasSuccessful &&
            a.CreatedAt >= sinceUtc &&
            a.StateFlag == StateFlags.ACTIVE);
    }
}
