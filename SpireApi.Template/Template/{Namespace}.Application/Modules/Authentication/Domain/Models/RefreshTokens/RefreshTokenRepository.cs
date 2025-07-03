using Microsoft.EntityFrameworkCore;
using {Namespace}.Application.Modules.Authentication.Infrastructure;
using {Namespace}.Shared.EntityFramework.Repositories;
using SpireCore.Constants;

namespace {Namespace}.Application.Modules.Authentication.Domain.Models.RefreshTokens;

public class RefreshTokenRepository : BaseRepository<RefreshToken, Guid, BaseAuthDbContext>
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

