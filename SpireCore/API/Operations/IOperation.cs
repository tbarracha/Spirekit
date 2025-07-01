namespace SpireCore.API.Operations;

public interface IOperation<TRequest, TResponse>
{
    Task<TResponse> ExecuteAsync(TRequest request);
}