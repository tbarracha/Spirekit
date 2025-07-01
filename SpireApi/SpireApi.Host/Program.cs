
using SpireCore.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOperations();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLifetimeServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", ctx =>
    {
        ctx.Response.Redirect("/swagger");
        return Task.CompletedTask;
    });
}

app.MapAllOperations();
OperationExtensions.ListAllOperations();

app.Run();
