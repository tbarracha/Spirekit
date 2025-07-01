
using SpireCore.API.Operations;
using SpireApi.Contracts.Hello;

namespace DddAutoOpsDemo.Application.Operations.Goodbye;

[OperationRoute("goodbye/world")]
[OperationMethod(OperationMethodType.DELETE)]
public class GoodbyeOperation : IOperation<HelloRequest, GoodbyeResponse>
{
    public Task<GoodbyeResponse> ExecuteAsync(HelloRequest request)
    {
        return Task.FromResult(new GoodbyeResponse
        {
            Message = $"Goodbye, {request.Name}!"
        });
    }
}
