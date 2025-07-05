using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Authentication.Domain.Models.AuthAudits;
using SpireApi.Application.Modules.Authentication.Domain.Models.AuthUserIdentities;
using SpireApi.Application.Modules.Authentication.Domain.Models.RefreshTokens;

namespace SpireApi.Application.Modules.Authentication.Infrastructure;

public class BaseAuthDbContext : IdentityDbContext<AuthUserIdentity, IdentityRole<Guid>, Guid>
{
    public BaseAuthDbContext(DbContextOptions options) : base(options) { }

    // === Identity Core ===
    public new DbSet<AuthUserIdentity> Users => Set<AuthUserIdentity>();
    public DbSet<AuthAudit> AuthAudits => Set<AuthAudit>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Register all IEntityTypeConfiguration<T> implementations (for all entities)
        builder.ApplyConfigurationsFromAssembly(typeof(BaseAuthDbContext).Assembly);
    }
}
