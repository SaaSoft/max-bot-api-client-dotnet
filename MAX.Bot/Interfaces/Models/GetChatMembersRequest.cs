using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces;

/// <summary>
/// Модель запроса на получение участников чата
/// </summary>
public class GetChatMembersRequest
{
    /// <summary>
    /// ID чата
    /// </summary>
    [JsonPropertyName("chatId")]
    public long? ChatId { get; set; }

    /// <summary>
    /// Список ID пользователей, чье членство нужно получить. 
    /// Когда этот параметр передан, параметры count и marker игнорируются
    /// </summary>
    [JsonPropertyName("user_ids")]
    public long[]? UserIds { get; set; }

    /// <summary>
    /// Указатель на следующую страницу данных
    /// </summary>
    [JsonPropertyName("marker")]
    public long? Marker { get; set; }

    /// <summary>
    /// По умолчанию: "20"
    /// Количество участников, которых нужно вернуть
    /// </summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }
}
