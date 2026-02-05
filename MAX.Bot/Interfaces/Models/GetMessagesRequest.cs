using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces;

/// <summary>
/// Модель запроса на получение сообщений
/// </summary>
public class GetMessagesRequest
{
    /// <summary>
    /// ID чата, чтобы получить сообщения из определённого чата. 
    /// Обязательный параметр, если не указан message_ids
    /// </summary>
    [JsonPropertyName("chat_id")]
    public long? ChatId { get; set; }

    /// <summary>
    /// Список ID сообщений, которые нужно получить (через запятую). 
    /// Обязательный параметр, если не указан chat_id
    /// </summary>
    [JsonPropertyName("message_ids")]
    public List<long>? MessageIds { get; set; }

    /// <summary>
    /// Время начала для запрашиваемых сообщений (в формате Unix timestamp)
    /// </summary>
    [JsonPropertyName("from")]
    public long? From { get; set; }

    /// <summary>
    /// Время окончания для запрашиваемых сообщений (в формате Unix timestamp)
    /// </summary>
    [JsonPropertyName("to")]
    public long? To { get; set; }

    /// <summary>
    /// По умолчанию: 50 
    /// Максимальное количество сообщений в ответе
    /// </summary>
    [JsonPropertyName("сount")]
    public int? Count { get; set; }
}