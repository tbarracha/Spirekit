
using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Features.Authentication;
using SpireApi.Application.Persistance;
using SpireApi.Host.Persistance;
using SpireCore.API.Services;
using SpireCore.API.Swagger;
using SpireCore.Events.Dispatcher;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi("Spire Auth API", "Spire Authentication API", "v1");

// Register DB context and IAM services
builder.Services.AddDbContext<BaseAuthDbContext, AuthBdContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AuthDb")));

builder.Services.AddDomainEventDispatcher();
builder.Services.AddOperations();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLifetimeServices();
builder.Services.AddAuth();


var app = builder.Build();

app.UseOpenApi(endpointName: "Spire Auth API", autoOpen: true);

app.MapAllOperations();
OperationExtensions.ListAllOperations();

app.Run();
