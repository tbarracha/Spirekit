using Microsoft.AspNetCore.Mvc;
using SpireApi.Shared.Operations;
using SpireApi.Shared.Operations.Attributes;
using System.Reflection;

public static class OperationEndpointMapperExtensions
{
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
}
