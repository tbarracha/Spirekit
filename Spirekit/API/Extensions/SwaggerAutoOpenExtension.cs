// -----------------------------------------------------------------------------
// Author: Tiago Barracha <ti.barracha@gmail.com>
// Created with AI assistance (ChatGPT)
// Description: Provides extension methods to register and launch Swagger UI
// -----------------------------------------------------------------------------
//
// USAGE:
//
// In your Program.cs or Startup.cs:
//
//   builder.Services.AddOpenApi("Your API Name", "Description", "v1");
//
//   app.UseOpenApi("Your API Name");
//   app.OpenSwaggerUI(); // Optional: Auto-opens Swagger in dev mode
//
// -----------------------------------------------------------------------------

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Diagnostics;

/// <summary>
/// Provides extension methods to configure and automatically launch Swagger UI for ASP.NET Core apps.
/// This utility supports configurable API metadata and optional JWT Bearer token input in Swagger.
/// Intended for use in development environments to quickly access and test HTTP APIs via browser.
/// </summary>
public static class SwaggerAutoOpenExtension
{
    /// <summary>
    /// Adds Swagger generation services with customizable title, description, and version.
    /// Also configures JWT Bearer token input for API testing.
    /// </summary>
    public static IServiceCollection AddOpenApi(
        this IServiceCollection services,
        string apiTitle = "API",
        string apiDescription = "API for this application",
        string apiVersion = "v1")
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(apiVersion, new OpenApiInfo
            {
                Title = apiTitle,
                Version = apiVersion,
                Description = apiDescription
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer {your JWT token}'"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Enables Swagger middleware and sets the Swagger UI endpoint and route prefix.
    /// </summary>
    public static void UseOpenApi(
        this IApplicationBuilder app,
        string endpointName = "API",
        string swaggerJsonPath = "/swagger/v1/swagger.json",
        string routePrefix = "")
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(swaggerJsonPath, endpointName);
            options.RoutePrefix = routePrefix; // default "" serves at root
        });
    }

    /// <summary>
    /// Automatically opens Swagger UI in the system's default browser after the app starts (in development only).
    /// </summary>
    public static void OpenSwaggerUI(
        this IApplicationBuilder app,
        int delayMs = 1500,
        string fallbackUrl = "http://localhost:5000")
    {
        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
        if (!env.IsDevelopment())
            return;

        Task.Run(async () =>
        {
            await Task.Delay(delayMs);

            var addresses = app.ServerFeatures
                .Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()
                ?.Addresses;

            var baseUrl = addresses?.FirstOrDefault() ?? fallbackUrl;
            var swaggerUrl = $"{baseUrl.TrimEnd('/')}/";

            try
            {
                Console.WriteLine($"Launching Swagger UI at {swaggerUrl}");
                Process.Start(new ProcessStartInfo
                {
                    FileName = swaggerUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to launch Swagger UI: {ex.Message}");
            }
        });
    }
}
