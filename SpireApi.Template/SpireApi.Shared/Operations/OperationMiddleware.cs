using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SpireApi.Shared.Operations;

public class OperationMiddleware
{
    private readonly ILogger<OperationMiddleware> _logger;

    public OperationMiddleware(ILogger<OperationMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> ExecuteAsync<TRequest, TResponse>(
        IOperation<TRequest, TResponse> operation,
        TRequest request)
    {
        try
        {
            return await operation.ExecuteAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in operation {OperationType}", operation.GetType().Name);

            // If TResponse is IActionResult, you can do:
            if (typeof(IActionResult).IsAssignableFrom(typeof(TResponse)))
            {
                var badResult = new BadRequestObjectResult(new
                {
                    error = "An error occurred while processing your request.",
                    details = ex.Message // Omit in production if needed
                });

                return (TResponse)(IActionResult)badResult;
            }

            // Otherwise, throw or return default/failure result
            throw;
        }
    }
}