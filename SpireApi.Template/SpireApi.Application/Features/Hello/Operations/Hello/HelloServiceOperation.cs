
using SpireCore.API.Operations;
using SpireApi.Contracts.Hello;
using SpireApi.Application.Features.Hello.Services;

namespace SpireApi.Application.Features.Hello.Operations.Hello;

[OperationGroup("Hello")]
[OperationRoute("hello/world/service")]
public class HelloServiceOperation : IOperation<HelloRequest, HelloResponse>
{
    private readonly IHelloService _helloService;

    public HelloServiceOperation(IHelloService helloService)
    {
        _helloService = helloService;
    }

    public Task<HelloResponse> ExecuteAsync(HelloRequest request)
    {
        return Task.FromResult(new HelloResponse
        {
            Message = _helloService.GetHello(request.Name)
        });
    }
}
