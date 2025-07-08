// -----------
// Auth
// -----------

// Create a new migration for AuthDbContext
dotnet ef migrations add <MigrationName> --context AuthDbContext --output-dir Migrations/Auth --project SpireApi.Infrastructure --startup-project SpireApi.Host


// Update the Auth database schema
dotnet ef database update --context AuthDbContext --project SpireApi.Infrastructure --startup-project SpireApi.Host

// List AuthDbContext migrations
dotnet ef migrations list --context AuthDbContext --project SpireApi.Infrastructure --startup-project SpireApi.Host

