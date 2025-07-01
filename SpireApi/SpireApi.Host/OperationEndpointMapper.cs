using Microsoft.AspNetCore.Mvc;
using SpireCore.API.Operations;
using System.Reflection;

public static class OperationEndpointMapper
{
    public static RouteHandlerBuilder MapOperation<TOp, TReq, TRes>(
    WebApplication app, string route, string method)
    where TOp : IOperation<TReq, TRes>
    {
        var groupAttr = typeof(TOp).GetCustomAttribute<OperationGroupAttribute>();

        var groupName = groupAttr?.GroupName
            ?? typeof(TOp).Namespace?.Split('.').Reverse().FirstOrDefault()
            ?? "misc";

        var builder = method switch
        {
            "GET" => app.MapGet(route, async ([FromServices] TOp op, [AsParameters] TReq req) =>
            {
                var result = await op.ExecuteAsync(req);
                return Results.Ok(result);
            }),

            "DELETE" => app.MapDelete(route, async ([FromServices] TOp op, [AsParameters] TReq req) =>
            {
                var result = await op.ExecuteAsync(req);
                return Results.Ok(result);
            }),

            "PUT" => app.MapPut(route, async ([FromServices] TOp op, [FromBody] TReq req) =>
            {
                var result = await op.ExecuteAsync(req);
                return Results.Ok(result);
            }),

            _ => app.MapPost(route, async ([FromServices] TOp op, [FromBody] TReq req) =>
            {
                var result = await op.ExecuteAsync(req);
                return Results.Ok(result);
            })
        };

        return builder;
    }

    public static bool IsSimpleQueryType(Type type)
    {
        if (type.IsPrimitive || type == typeof(string) || type == typeof(Guid))
            return true;

        if (!type.IsClass || type == typeof(string)) return false;

        return type.GetProperties().All(p =>
        {
            var pt = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
            return pt.IsPrimitive || pt == typeof(string) || pt == typeof(Guid);
        });
    }
}
