// -----------------------------------------------------------------------------
// Author: Tiago Barracha <ti.barracha@gmail.com>
// Created with AI assistance (ChatGPT)
//
// Description: 
// -----------------------------------------------------------------------------

namespace SpireApi.Shared.Operations;

public interface IOperation<TRequest, TResponse>
{
    Task<TResponse> ExecuteAsync(TRequest request);
}