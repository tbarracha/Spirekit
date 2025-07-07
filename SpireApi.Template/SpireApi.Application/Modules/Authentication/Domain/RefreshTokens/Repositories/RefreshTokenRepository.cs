using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Authentication.Domain.RefreshTokens.Models;
using SpireApi.Application.Modules.Authentication.Infrastructure;
using SpireCore.API.EntityFramework.Entities.Repositories;
using SpireCore.Constants;

namespace SpireApi.Application.Modules.Authentication.Domain.RefreshTokens.Repositories;

public class RefreshTokenRepository : BaseEntityRepository<RefreshToken, Guid, BaseAuthDbContext>
{
    public RefreshTokenRepository(BaseAuthDbContext context) : base(context) { }

    public async Task<RefreshToken?> GetValidTokenAsync(string token)
    {
        return await _dbSet.Include(r => r.AuthUser)
                           .FirstOrDefaultAsync(r =>
                               r.Token == token &&
                               !r.IsRevoked &&
                               r.ExpiresAt > DateTime.UtcNow &&
                               r.StateFlag == StateFlags.ACTIVE);
    }

    public async Task RevokeTokenAsync(RefreshToken token)
    {
        token.IsRevoked = true;
        token.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(token);
        await _context.SaveChangesAsync();
    }
}
