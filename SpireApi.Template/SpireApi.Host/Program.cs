using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Authentication;
using SpireApi.Application.Modules.Authentication.Configuration;
using SpireApi.Application.Modules.Authentication.Infrastructure;
using SpireApi.Application.Modules.Iam;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups.Repositories;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Application.Shared.Entities;
using SpireApi.Infrastructure.Authentication;
using SpireApi.Infrastructure.Iam;
using SpireApi.Shared.JWT;
using SpireApi.Shared.JWT.MicroServiceIdentity;
using SpireApi.Shared.JWT.UserIdentity;
using SpireApi.Shared.Services;
using SpireApi.Shared.Swagger;
using SpireCore.Events.Dispatcher;

var builder = WebApplication.CreateBuilder(args);

// --- Core & app-specific services ---
builder.Services.AddOpenApi("Spire Auth API", "Spire Authentication API", "v1");
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddServiceIdentityAndTokenProvider(builder.Configuration);

// --- Module-based connection strings
var authConnString = builder.Configuration["Modules:Auth:ConnectionString"]
    ?? "Host=localhost;Port=5432;Database=spire_auth_db;Username=postgres;Password=postgres";

builder.Services.AddDbContext<BaseAuthDbContext, AuthDbContext>(options =>
    options.UseNpgsql(authConnString));

var iamConnString = builder.Configuration["Modules:Iam:ConnectionString"]
    ?? "Host=localhost;Port=5432;Database=spire_iam_db;Username=postgres;Password=postgres";

builder.Services.AddDbContext<BaseIamDbContext, IamDbContext>(options =>
    options.UseNpgsql(iamConnString));

builder.Services.AddScoped<GuidEntityDbContext>(sp => sp.GetRequiredService<IamDbContext>());
builder.Services.AddScoped<BaseIamDbContext>(sp => sp.GetRequiredService<IamDbContext>());



builder.Services.AddDomainEventDispatcher();

// --- Centralized authentication/authorization/identity ---
builder.Services.AddAuthModuleServices();
builder.Services.AddIamModuleServices();
builder.Services.AddUnifiedJwtAuthentication(builder.Configuration); // For all JWT config!

// --- Map Operations & Create Endpoints
builder.Services.AddOperations();
builder.Services.AddLifetimeServices();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- Conditionally enable authentication and authorization middleware ---
var authSettings = builder.Configuration.GetSection("AuthSettings").Get<AuthSettings>() ?? new AuthSettings();

if (authSettings.Authentication)
{
    app.UseAuthentication();
    app.UseAuthorization();
}

app.UseOpenApi(endpointName: "Spire Auth API", autoOpen: true);
app.MapAllOperations();
OperationExtensions.ListAllOperations();

app.Run();
