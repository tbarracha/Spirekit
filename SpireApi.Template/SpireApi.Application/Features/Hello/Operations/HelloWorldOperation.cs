
using SpireCore.API.Operations;
using SpireApi.Contracts.Dtos.Features.Hello;

namespace SpireApi.Application.Features.Hello.Operations;

[OperationGroup("Hello")]
[OperationRoute("hello/world")]
public class HelloWorldOperation : IOperation<HelloRequest, HelloResponse>
{
    public Task<HelloResponse> ExecuteAsync(HelloRequest request)
    {
        return Task.FromResult(new HelloResponse
        {
            Message = $"Hello, {request.Name} {request.LastName}!"
        });
    }
}
