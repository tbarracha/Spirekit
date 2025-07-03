
using {Namespace}.Contracts.Dtos.Features.Hello;
using {Namespace}.Shared.Operations;
using {Namespace}.Shared.Operations.Attributes;

namespace {Namespace}.Application.Features.Hello.Operations;

[OperationGroup("Hello")]
[OperationRoute("hello/world")]
public class HelloWorldOperation : IOperation<HelloRequestDto, HelloResponseDto>
{
    public Task<HelloResponseDto> ExecuteAsync(HelloRequestDto request)
    {
        return Task.FromResult(new HelloResponseDto
        {
            Message = $"Hello, {request.Name} {request.LastName}!"
        });
    }
}

