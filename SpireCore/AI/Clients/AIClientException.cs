using System.Net;

namespace SpireCore.AI.Clients;

public class AIClientException : Exception
{
    public HttpStatusCode? StatusCode { get; }
    public string? ResponseContent { get; }

    public AIClientException(string message)
        : base(message) { }

    public AIClientException(string message, Exception innerException)
        : base(message, innerException) { }

    public AIClientException(string message, HttpStatusCode statusCode, string? responseContent = null)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }

    public AIClientException(string message, HttpStatusCode statusCode, string? responseContent, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }
}