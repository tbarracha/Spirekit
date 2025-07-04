using System;
using System.Threading.Tasks;

namespace SpireApi.Shared.Operations;

public abstract class BaseOperation<TRequest, TResponse> : IOperation<TRequest, TResponse>
{
    public virtual async Task<TResponse> ExecuteAsync(TRequest request)
    {
        try
        {
            return await ExecuteOperationAsync(request);
        }
        catch (Exception ex)
        {
            throw; // Or optionally, wrap/return a failure response
        }
    }

    protected abstract Task<TResponse> ExecuteOperationAsync(TRequest request);
}
