
using SpireCore.API.Operations;
using SpireApi.Application.Features.Hello.Services;
using SpireApi.Contracts.Dtos.Features.Hello;
using SpireCore.API.Operations.Attributes;

namespace SpireApi.Application.Features.Hello.Operations;

[OperationGroup("Hello")]
[OperationRoute("hello/world/service")]
public class HelloServiceOperation : IOperation<HelloRequestDto, HelloResponseDto>
{
    private readonly IHelloService _helloService;

    public HelloServiceOperation(IHelloService helloService)
    {
        _helloService = helloService;
    }

    public Task<HelloResponseDto> ExecuteAsync(HelloRequestDto request)
    {
        return Task.FromResult(new HelloResponseDto
        {
            Message = _helloService.GetHello(request.Name)
        });
    }
}
