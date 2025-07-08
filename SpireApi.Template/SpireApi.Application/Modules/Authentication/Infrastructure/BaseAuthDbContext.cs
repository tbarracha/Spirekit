using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Authentication.Domain.AuthAudits.Models;
using SpireApi.Application.Modules.Authentication.Domain.AuthUserIdentities;
using SpireApi.Application.Modules.Authentication.Domain.RefreshTokens.Models;
using SpireCore.API.EntityFramework.DbContexts;

namespace SpireApi.Application.Modules.Authentication.Infrastructure;

public class BaseAuthDbContext : IdentityDbContext<AuthUserIdentity, IdentityRole<Guid>, Guid>
{
    public BaseAuthDbContext(DbContextOptions options) : base(options) { }

    // === Identity Core ===
    public new DbSet<AuthUserIdentity> Users => Set<AuthUserIdentity>();
    public DbSet<AuthAudit> AuthAudits => Set<AuthAudit>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureEnumStorageAsString();
        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        modelBuilder.ApplyIEntityConfiguration();
    }
}
