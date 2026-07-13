namespace MAX.Bot.Interfaces.Models.Attachment;

/// <summary>
/// Параметры повторных попыток при работе с вложениями.
/// </summary>
public class AttachmentRetryOptions
{
    /// <summary>
    /// Максимальное количество попыток.
    /// </summary>
    public int MaxAttempts { get; set; } = 4;

    /// <summary>
    /// Пауза между повторными попытками. По умолчанию — 2 секунды.
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(2);
}