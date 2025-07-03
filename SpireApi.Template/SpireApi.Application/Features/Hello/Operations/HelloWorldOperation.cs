
using SpireApi.Contracts.Dtos.Features.Hello;
using SpireApi.Shared.Operations;
using SpireApi.Shared.Operations.Attributes;

namespace SpireApi.Application.Features.Hello.Operations;

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
