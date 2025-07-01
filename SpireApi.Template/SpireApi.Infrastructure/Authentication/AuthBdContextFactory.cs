using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SpireApi.Infrastructure.Authentication
{
    public class AuthBdContextFactory : IDesignTimeDbContextFactory<AuthBdContext>
    {
        public AuthBdContext CreateDbContext(string[] args)
        {
            // Look for appsettings.json in current and parent dirs (useful for running from either Infra or root)
            string basePath = Directory.GetCurrentDirectory();
            string jsonPath = "appsettings.json";
            if (!File.Exists(Path.Combine(basePath, jsonPath)))
            {
                // Try up one level (common if running from Infrastructure folder)
                basePath = Path.Combine(basePath, "..", "SpireApi.Host");
            }

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AuthBdContext>();

            // Use the correct key and default exactly as in your posted appsettings
            var connectionString = config.GetConnectionString("AuthDb")
                ?? "Host=localhost;Port=5432;Database=spire_auth_db;Username=postgres;Password=postgres";

            optionsBuilder.UseNpgsql(connectionString);

            return new AuthBdContext(optionsBuilder.Options);
        }
    }
}
