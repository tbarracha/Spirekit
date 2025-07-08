using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using SpireCore.API.Operations;
using SpireCore.API.Operations.Attributes;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using System.Text;

public static class OperationsExtensions
{
    public static void AddOperations(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(c => c.AssignableTo(typeof(IOperation<,>)))
            .AsSelf()
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.AddScoped<OperationMiddleware>();
    }

    public static RouteHandlerBuilder MapOperation<TOp, TReq, TRes>(
        WebApplication app, string route, string method)
        where TOp : IOperation<TReq, TRes>
    {
        var groupAttr = typeof(TOp).GetCustomAttribute<OperationGroupAttribute>();
        var groupName = OperationGroupAttribute.GetGroupName(typeof(TOp));

        var builder = method switch
        {
            "GET" => app.MapGet(route, async (
                [FromServices] OperationMiddleware middleware,
                [FromServices] TOp op,
                [AsParameters] TReq req) =>
            {
                return await middleware.ExecuteAsync(op, req);
            }),

            "DELETE" => app.MapDelete(route, async (
                [FromServices] OperationMiddleware middleware,
                [FromServices] TOp op,
                [AsParameters] TReq req) =>
            {
                return await middleware.ExecuteAsync(op, req);
            }),

            "PUT" => app.MapPut(route, async (
                [FromServices] OperationMiddleware middleware,
                [FromServices] TOp op,
                [FromBody] TReq req) =>
            {
                return await middleware.ExecuteAsync(op, req);
            }),

            _ => app.MapPost(route, async (
                [FromServices] OperationMiddleware middleware,
                [FromServices] TOp op,
                [FromBody] TReq req) =>
            {
                return await middleware.ExecuteAsync(op, req);
            })
        };

        // -- Check for file attribute --
        var producesFileAttr = typeof(TOp).GetCustomAttribute<OperationProducesFileAttribute>();
        if (producesFileAttr is not null)
        {
            builder.Produces(
                StatusCodes.Status200OK,
                typeof(FileResult),        // This is correct for files (or use typeof(void) for no body)
                producesFileAttr.ContentType
            );

            // Optional: Add OpenApi doc hint
            builder.WithOpenApi(op =>
            {
                op.Responses["200"].Description = $"Returns a file download ({producesFileAttr.ContentType})";
                op.Responses["200"].Content.Clear();
                op.Responses["200"].Content.Add(producesFileAttr.ContentType, new Microsoft.OpenApi.Models.OpenApiMediaType());
                return op;
            });
        }
        else
        {
            // Default: normal typed response
            builder.Produces<TRes>(StatusCodes.Status200OK);
            builder.WithOpenApi(op =>
            {
                op.Responses["200"].Description = $"Success ({typeof(TRes).Name})";
                return op;
            });
        }

        return builder;
    }

    /// <summary>
    /// Returns only assemblies relevant to Spire operation discovery.
    /// </summary>
    private static IEnumerable<Assembly> GetRelevantAssemblies()
    {
        // Adjust prefix if needed (e.g. "Spire", "SpireApi", etc.)
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a =>
                a.GetName().Name != null &&
                (a.GetName().Name.StartsWith("Spire"))
            );
    }

    /// <summary>
    /// Gets a group name for an operation type, prioritizing attribute and then namespace segment.
    /// </summary>
    private static string GetOperationGroupName(Type type)
        => OperationGroupAttribute.GetGroupName(type);

    public static void MapAllOperations(this WebApplication app)
    {
        try
        {
            var relevantAssemblies = GetRelevantAssemblies();

            var opTypes = new List<Type>();
            foreach (var assembly in relevantAssemblies)
            {
                try
                {
                    opTypes.AddRange(
                        assembly.GetTypes().Where(t =>
                            t.IsClass &&
                            !t.IsAbstract &&
                            (t.Namespace?.Contains(".Operations.") == true || t.Namespace?.Contains(".Operations") == true) &&
                            t.GetInterfaces().Any(i =>
                                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IOperation<,>)))
                    );
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.WriteLine($"[ReflectionTypeLoadException] Failed to load types from assembly: {assembly.FullName}");
                    foreach (var loaderEx in ex.LoaderExceptions ?? Array.Empty<Exception>())
                    {
                        if (loaderEx != null)
                        {
                            Console.WriteLine("    [LoaderException] " + loaderEx.GetType().FullName + ": " + loaderEx.Message);
                            if (loaderEx is FileNotFoundException fnf)
                            {
                                Console.WriteLine("        FileName: " + fnf.FileName);
                                Console.WriteLine("        FusionLog: " + fnf.FusionLog);
                            }
                        }
                    }
                    // continue; (skip this assembly)
                }
            }

            foreach (var opType in opTypes)
            {
                try
                {
                    var iface = opType.GetInterfaces()
                        .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IOperation<,>));
                    if (iface is null) continue;

                    var requestType = iface.GetGenericArguments()[0];
                    var responseType = iface.GetGenericArguments()[1];

                    var routeAttr = opType.GetCustomAttribute<OperationRouteAttribute>();
                    var methodAttr = opType.GetCustomAttribute<OperationMethodAttribute>();
                    var authAttr = opType.GetCustomAttribute<OperationAuthorizeAttribute>();

                    var groupName = GetOperationGroupName(opType);
                    var operationName = opType.Name.Replace("Operation", "").ToLower();

                    var route = routeAttr != null
                        ? $"/api/{routeAttr.Route}"
                        : $"/api/{groupName.ToLower()}/{operationName}";

                    var httpMethod = methodAttr?.HttpMethod ?? "POST";

                    var method = typeof(OperationsExtensions)
                        .GetMethod(nameof(OperationsExtensions.MapOperation), BindingFlags.Public | BindingFlags.Static)!
                        .MakeGenericMethod(opType, requestType, responseType);

                    var endpointBuilder = (RouteHandlerBuilder)method.Invoke(null, [app, route, httpMethod])!;

                    if (authAttr != null)
                    {
                        if (authAttr.Policy is not null)
                            endpointBuilder.RequireAuthorization(authAttr.Policy);
                        else
                            endpointBuilder.RequireAuthorization();
                    }

                    //Console.WriteLine($"[DEBUG] Mapping {opType.FullName} with route {route} {(authAttr != null ? "REQUIRES AUTH" : "NO AUTH")}");
                    endpointBuilder.WithTags(groupName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] Failed to map operation for type {opType.FullName}: {ex}");
                    // continue
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[TopLevelError] Unexpected error in MapAllOperations: " + ex);
            throw;
        }
    }

    public static void ListAllOperations()
    {
        try
        {
            var relevantAssemblies = GetRelevantAssemblies();
            var opTypes = new List<Type>();
            foreach (var assembly in relevantAssemblies)
            {
                try
                {
                    opTypes.AddRange(
                        assembly.GetTypes().Where(t =>
                            t.IsClass &&
                            !t.IsAbstract &&
                            (t.Namespace?.Contains(".Operations.") == true || t.Namespace?.Contains(".Operations") == true) &&
                            t.GetInterfaces().Any(i =>
                                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IOperation<,>)))
                    );
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.WriteLine($"[ReflectionTypeLoadException] Failed to load types from assembly: {assembly.FullName}");
                    foreach (var loaderEx in ex.LoaderExceptions ?? Array.Empty<Exception>())
                    {
                        if (loaderEx != null)
                        {
                            Console.WriteLine("    [LoaderException] " + loaderEx.GetType().FullName + ": " + loaderEx.Message);
                            if (loaderEx is FileNotFoundException fnf)
                            {
                                Console.WriteLine("        FileName: " + fnf.FileName);
                                Console.WriteLine("        FusionLog: " + fnf.FusionLog);
                            }
                        }
                    }
                }
            }

            Console.WriteLine(">> Registered operations:");
            foreach (var type in opTypes)
            {
                try
                {
                    var routeAttr = type.GetCustomAttribute<OperationRouteAttribute>();
                    var methodAttr = type.GetCustomAttribute<OperationMethodAttribute>();

                    var groupName = GetOperationGroupName(type);

                    var operationName = type.Name.Replace("Operation", "").ToLower();
                    var route = routeAttr?.Route ?? $"{groupName.ToLower()}/{operationName}";
                    var method = methodAttr?.HttpMethod ?? "POST";

                    Console.WriteLine($" - [{method}] /api/{route} => {type.FullName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] Failed to list operation for type {type.FullName}: {ex}");
                }
            }
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine("[TopLevelError] Unexpected error in ListAllOperations: " + ex);
            throw;
        }
    }
}
