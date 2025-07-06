using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SpireApi.Application.Features;
using SpireApi.Application.Modules;
using SpireApi.Application.Modules.Authentication.Configuration;
using SpireApi.Application.Modules.Authentication.Infrastructure;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Infrastructure.Authentication;
using SpireApi.Infrastructure.Iam;
using SpireApi.Shared.Configuration.Features;
using SpireApi.Shared.Configuration.Modules;
using SpireApi.Shared.JWT;
using SpireApi.Shared.JWT.Identity.Users;
using SpireApi.Shared.Swagger;
using SpireCore.Events.Dispatcher;
using SpireCore.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Core & app-specific services ---
builder.Services.AddOpenApi("Spire Auth API", "Spire Authentication API", "v1");
builder.Services.AddSingleton<IJwtUserService, JwtUserService>();

// --- Module & Feature Configuration Binding ---
builder.Services.Configure<ModulesConfigurationList>(builder.Configuration.GetSection("Modules"));
builder.Services.Configure<FeaturesConfigurationList>(builder.Configuration.GetSection("Features"));

// --- Temporary service provider to resolve options ---
using var tempProvider = builder.Services.BuildServiceProvider();
var modulesConfig = tempProvider.GetRequiredService<IOptions<ModulesConfigurationList>>().Value;

// --- Resolve module connection strings with fallback ---
var authConfig = modulesConfig.TryGetValue("Auth", out var ac) ? ac : new ModuleConfiguration();
var authConnString = authConfig.DbConnectionString
    ?? "Host=localhost;Port=5432;Database=spire_auth_db;Username=postgres;Password=postgres";

var iamConfig = modulesConfig.TryGetValue("Iam", out var ic) ? ic : new ModuleConfiguration();
var iamConnString = iamConfig.DbConnectionString
    ?? "Host=localhost;Port=5432;Database=spire_iam_db;Username=postgres;Password=postgres";

// --- DB Contexts ---
builder.Services.AddDbContext<BaseAuthDbContext, AuthDbContext>(options =>
    options.UseNpgsql(authConnString));

builder.Services.AddDbContext<BaseIamDbContext, IamDbContext>(options =>
    options.UseNpgsql(iamConnString));

// --- Event dispatcher, modules & domain services ---
builder.Services.AddDomainEventDispatcher();
builder.Services.FilterEnabledModules(modulesConfig);
builder.Services.AddJwtAuthentication(builder.Configuration);

var featuresConfig = tempProvider.GetRequiredService<IOptions<FeaturesConfigurationList>>().Value;
builder.Services.AddEnabledFeatures(featuresConfig);

builder.Services.AddOperations();
builder.Services.AddApplicationServices();

// --- Swagger & API Explorer ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- Middleware & Routing ---
var authSettings = builder.Configuration.GetSection("AuthSettings").Get<AuthSettings>() ?? new AuthSettings();

if (authSettings.Authentication)
{
    app.UseAuthentication();
    app.UseAuthorization();
}

app.UseOpenApi(endpointName: "Spire Auth API", autoOpen: true);
app.MapAllOperations();

app.Run();
