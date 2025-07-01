using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Domain.AppUsers.Models;
using SpireApi.Application.Domain.AuthAudit;

namespace SpireApi.Application.Persistance;

public abstract class BaseAuthDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    protected BaseAuthDbContext(DbContextOptions options) : base(options) { }

    // === Identity Core ===
    public new DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<AuthAudit> AuthAudits => Set<AuthAudit>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Register all IEntityTypeConfiguration<T> implementations (for all entities)
        builder.ApplyConfigurationsFromAssembly(typeof(AppUser).Assembly);
    }
}
