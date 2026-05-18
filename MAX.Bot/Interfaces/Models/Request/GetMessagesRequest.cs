using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models.Request;

/// <summary>
/// Модель запроса на получение сообщений
/// </summary>
public record GetMessagesRequest
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
    public List<string>? MessageIds { get; set; }

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
    [JsonPropertyName("count")]
    public int? Count { get; set; }

    /// <summary>
    /// Проверить параметры запроса сообщений
    /// </summary>
    public void Validate()
    {
        var hasChatId = ChatId.HasValue;
        var hasMessageIds = MessageIds is { Count: > 0 };

        if (hasChatId == hasMessageIds)
            throw new ArgumentException("Нужно указать ровно один параметр: chat_id или message_ids.");

        if (MessageIds != null && MessageIds.Exists(string.IsNullOrWhiteSpace))
            throw new ArgumentException("message_ids не должен содержать пустые идентификаторы.", nameof(MessageIds));

        if (Count is < 1 or > 100)
            throw new ArgumentOutOfRangeException(nameof(Count), Count, "count должен быть в диапазоне от 1 до 100.");

        if (From.HasValue && To.HasValue && From.Value > To.Value)
            throw new ArgumentException("from не должен быть больше to.");
    }
}
