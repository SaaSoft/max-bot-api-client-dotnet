using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models.Request;

/// <summary>
/// Модель запроса на отправку сообщения
/// </summary>
public record SendMessageRequest
{
    /// <summary>
    /// ID пользователя для отправки личного сообщения
    /// </summary>
    [JsonPropertyName("user_id")]
    public long? UserId { get; set; }

    /// <summary>
    /// ID чата для отправки сообщения в группу
    /// </summary>
    [JsonPropertyName("chat_id")]
    public long? ChatId { get; set; }

    /// <summary>
    /// Отключение предпросмотра ссылок в сообщении
    /// </summary>
    [JsonPropertyName("disable_link_preview")]
    public bool? DisableLinkPreview { get; set; }

    /// <summary>
    /// Текст сообщения (до 4000 символов)
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// Уведомлять ли участников чата о сообщении
    /// По умолчанию: true
    /// </summary>
    [JsonPropertyName("notify")]
    public bool? Notify { get; set; } = true;

    /// <summary>
    /// Форматирование текста сообщения
    /// </summary>
    [JsonPropertyName("format")]
    public MessageFormat? Format { get; set; }
}
