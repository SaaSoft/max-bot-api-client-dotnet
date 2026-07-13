using System.Net;

namespace MAX.Bot.Exceptions;

public class MaxBotClientException : Exception
{
    private readonly bool _isAttachmentNotReady;

    public HttpStatusCode StatusCode { get; }

    public MaxBotClientException(
        string message,
        HttpStatusCode statusCode,
        bool isAttachmentNotReady = false)
        : base(message)
    {
        StatusCode = statusCode;
        _isAttachmentNotReady = isAttachmentNotReady;
    }

    /// <summary>
    /// Вложение ещё обрабатывается на стороне MAX и пока недоступно.
    /// </summary>
    public bool IsAttachmentNotReady =>
        _isAttachmentNotReady ||
        Message.IndexOf("attachment.not.ready", StringComparison.OrdinalIgnoreCase) >= 0 ||
        Message.IndexOf("not.processed", StringComparison.OrdinalIgnoreCase) >= 0;
}