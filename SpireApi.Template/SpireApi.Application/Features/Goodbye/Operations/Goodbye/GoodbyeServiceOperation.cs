
using SpireCore.API.Operations;
using SpireApi.Contracts.Hello;
using SpireApi.Application.Features.Goodbye.Services;

namespace SpireApi.Application.Features.Goodbye.Operations.Goodbye;

[OperationRoute("goodbye/world/service")]
public class GoodbyeServiceOperation : IOperation<HelloRequest, HelloResponse>
{
    private readonly GoodbyeService _goodbyeService;

    public GoodbyeServiceOperation(GoodbyeService goodbyeService)
    {
        _goodbyeService = goodbyeService;
    }

    public Task<HelloResponse> ExecuteAsync(HelloRequest request)
    {
        return Task.FromResult(new HelloResponse
        {
            Message = _goodbyeService.GetHello(request.Name)
        });
    }
}
