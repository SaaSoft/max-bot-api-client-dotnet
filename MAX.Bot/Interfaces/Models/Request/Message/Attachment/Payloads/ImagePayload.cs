namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment.Payloads;

/// <summary>
/// Запрос на прикрепление изображения (все поля являются взаимоисключающими)
/// </summary>
public sealed class ImagePayload : AttachmentPayload
{
    /// <summary>
    /// от 1 символа
    /// Любой внешний URL изображения, которое вы хотите прикрепить
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }

    /// <summary>
    /// Токен существующего вложения
    /// </summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>
    /// Токены, полученные после загрузки изображений
    /// </summary>
    [JsonPropertyName("photos")]
    public string? Photos { get; set; }
}