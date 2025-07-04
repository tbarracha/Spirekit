using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SpireApi.Infrastructure.Iam
{
    public class IamDbContextFactory : IDesignTimeDbContextFactory<IamDbContext>
    {
        public IamDbContext CreateDbContext(string[] args)
        {
            string basePath = Directory.GetCurrentDirectory();
            string jsonPath = "appsettings.json";
            if (!File.Exists(Path.Combine(basePath, jsonPath)))
            {
                basePath = Path.Combine(basePath, "..", "SpireApi.Host");
            }

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<IamDbContext>();

            // Get connection string from Modules section
            var connectionString = config.GetSection("Modules:Iam:ConnectionString").Value
                ?? "Host=localhost;Port=5432;Database=spire_iam_db;Username=postgres;Password=postgres";

            optionsBuilder.UseNpgsql(connectionString);

            return new IamDbContext(optionsBuilder.Options);
        }
    }
}
