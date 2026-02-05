using System.Net;

namespace MAX.Bot.Exceptions;

public class MaxBotClientException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public MaxBotClientException(string message, HttpStatusCode statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }
}