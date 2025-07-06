using Microsoft.EntityFrameworkCore;

namespace SpireApi.Shared.EntityFramework.DbContexts;

public abstract class BaseEntityDbContext : DbContext
{
    protected BaseEntityDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        modelBuilder.ApplyIEntityConfiguration();
    }
}
