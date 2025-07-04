using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Authentication.Domain.Models.AuthAudits;
using SpireApi.Application.Modules.Authentication.Domain.Models.AuthUsers;
using SpireApi.Application.Modules.Authentication.Domain.Models.RefreshTokens;

namespace SpireApi.Application.Modules.Authentication.Infrastructure;

public abstract class BaseAuthDbContext : IdentityDbContext<AuthUser, IdentityRole<Guid>, Guid>
{
    protected BaseAuthDbContext(DbContextOptions options) : base(options) { }

    // === Identity Core ===
    public new DbSet<AuthUser> Users => Set<AuthUser>();
    public DbSet<AuthAudit> AuthAudits => Set<AuthAudit>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Register all IEntityTypeConfiguration<T> implementations (for all entities)
        builder.ApplyConfigurationsFromAssembly(typeof(BaseAuthDbContext).Assembly);
    }
}
