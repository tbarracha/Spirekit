using Microsoft.EntityFrameworkCore;
using {Namespace}.Application.Modules.Authentication.Configuration;
using {Namespace}.Application.Modules.Authentication.Infrastructure;
using {Namespace}.Infrastructure.Authentication;
using {Namespace}.Shared.JWT;
using {Namespace}.Shared.JWT.UserIdentity;
using {Namespace}.Shared.JWT.MicroServiceIdentity;
using SpireCore.Events.Dispatcher;
using {Namespace}.Shared.Services;
using {Namespace}.Shared.Swagger;

var builder = WebApplication.CreateBuilder(args);

// --- Core & app-specific services ---
builder.Services.AddOpenApi("Spire Auth API", "Spire Authentication API", "v1");
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddServiceIdentityAndTokenProvider(builder.Configuration);

builder.Services.AddDbContext<BaseAuthDbContext, AuthBdContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AuthDb")));

builder.Services.AddDomainEventDispatcher();
builder.Services.AddOperations();
builder.Services.AddLifetimeServices();
builder.Services.AddEndpointsApiExplorer();

// --- Centralized authentication/authorization/identity ---
builder.Services.AddDomainAuthServices();
builder.Services.AddUnifiedJwtAuthentication(builder.Configuration); // For all JWT config!

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

