namespace {Namespace}.Shared.Operations;

public interface IOperation<TRequest, TResponse>
{
    Task<TResponse> ExecuteAsync(TRequest request);
}
