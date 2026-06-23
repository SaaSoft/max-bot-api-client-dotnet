using MAX.Bot.Interfaces.Models.Request.Message.Attachment;

namespace MAX.Bot.Interfaces.Models;

/// <summary>
/// Модель тела сообщения
/// </summary>
public record MessageBody
{
    /// <summary>
    /// Уникальный ID сообщения
    /// </summary>
    [JsonPropertyName("mid")]
    public string? Mid { get; set; }

    /// <summary>
    /// Новый текст сообщения
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// Вложения
    /// </summary>
    [JsonPropertyName("attachments")]
    public Attachment[]? Attachments { get; set; }
}