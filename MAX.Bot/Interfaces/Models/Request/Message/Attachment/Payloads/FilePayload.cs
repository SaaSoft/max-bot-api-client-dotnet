namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment.Payloads;

/// <summary>
/// Это информация, которую вы получите, как только аудио/видео будет загружено
/// </summary>
public sealed class FilePayload: AttachmentPayload
{
    /// <summary>
    /// URL загруженного файла
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Токен — уникальный ID загруженного медиафайла
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; set; }
}