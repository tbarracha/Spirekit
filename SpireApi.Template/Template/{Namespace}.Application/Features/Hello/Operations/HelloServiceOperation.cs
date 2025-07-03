
using {Namespace}.Application.Features.Hello.Services;
using {Namespace}.Contracts.Dtos.Features.Hello;
using {Namespace}.Shared.Operations;
using {Namespace}.Shared.Operations.Attributes;

namespace {Namespace}.Application.Features.Hello.Operations;

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

