using SpireCore.API.Operations;
using System.Reflection;

public static class OperationExtensions
{
    public static void AddOperations(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(c => c.AssignableTo(typeof(IOperation<,>)))
            .AsSelf()
            .AsImplementedInterfaces()
            .WithTransientLifetime());
    }

    public static void MapAllOperations(this WebApplication app)
    {
        var opTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                (
                    (t.Namespace?.Contains(".Operations.") == true) ||
                    (t.Namespace?.Contains(".Operations") == true)
                ) &&
                t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IOperation<,>))
            )
            .ToList();

        foreach (var opType in opTypes)
        {
            var iface = opType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IOperation<,>));
            if (iface is null) continue;

            var requestType = iface.GetGenericArguments()[0];
            var responseType = iface.GetGenericArguments()[1];

            var routeAttr = opType.GetCustomAttribute<OperationRouteAttribute>();
            var methodAttr = opType.GetCustomAttribute<OperationMethodAttribute>();
            var groupAttr = opType.GetCustomAttribute<OperationGroupAttribute>();
            var authAttr = opType.GetCustomAttribute<OperationAuthorizeAttribute>();

            var groupName = OperationGroupAttribute.GetGroupName(opType);

            var operationName = opType.Name.Replace("Operation", "").ToLower();

            var route = routeAttr != null
                ? $"/api/{routeAttr.Route}"
                : $"/api/{groupName.ToLower()}/{operationName}";

            var httpMethod = methodAttr?.HttpMethod ?? "POST";

            var method = typeof(OperationEndpointMapper)
                .GetMethod(nameof(OperationEndpointMapper.MapOperation), BindingFlags.Public | BindingFlags.Static)!
                .MakeGenericMethod(opType, requestType, responseType);

            var endpointBuilder = (RouteHandlerBuilder)method.Invoke(null, [app, route, httpMethod])!;

            if (authAttr != null)
            {
                if (authAttr.Policy is not null)
                    endpointBuilder.RequireAuthorization(authAttr.Policy);
                else
                    endpointBuilder.RequireAuthorization();
            }

            endpointBuilder.WithTags(groupName);
        }
    }

    public static void ListAllOperations()
    {
        var opTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                (
                    (t.Namespace?.Contains(".Operations.") == true) ||
                    (t.Namespace?.Contains(".Operations") == true)
                ) &&
                t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IOperation<,>))
            )
            .ToList();

        Console.WriteLine(">> Registered operations:");
        foreach (var type in opTypes)
        {
            var groupAttr = type.GetCustomAttribute<OperationGroupAttribute>();
            var routeAttr = type.GetCustomAttribute<OperationRouteAttribute>();
            var methodAttr = type.GetCustomAttribute<OperationMethodAttribute>();

            var groupName = OperationGroupAttribute.GetGroupName(type);

            var operationName = type.Name.Replace("Operation", "").ToLower();
            var route = routeAttr?.Route ?? $"{groupName.ToLower()}/{operationName}";
            var method = methodAttr?.HttpMethod ?? "POST";

            Console.WriteLine($" - [{method}] /api/{route} => {type.FullName}");
        }

        Console.WriteLine();
    }
}
