using Microsoft.EntityFrameworkCore;

namespace SpireApi.Shared.EntityFramework.DbContexts;

public abstract class BaseDbContext : DbContext
{
    protected BaseDbContext(DbContextOptions options) : base(options) { }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureEnumStorageAsString();
        base.ConfigureConventions(configurationBuilder);
    }
}
