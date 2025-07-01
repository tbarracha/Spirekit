
using SpireCore.API.Operations;
using SpireApi.Contracts.Hello;

namespace DddAutoOpsDemo.Application.Operations.Hello;

[OperationGroup("hello")]
public class HelloWorldOperation : IOperation<HelloRequest, HelloResponse>
{
    public Task<HelloResponse> ExecuteAsync(HelloRequest request)
    {
        return Task.FromResult(new HelloResponse
        {
            Message = $"Hello, {request.Name}!"
        });
    }
}
