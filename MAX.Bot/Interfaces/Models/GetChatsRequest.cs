using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces;

/// <summary>
/// Модель запроса на получение чатов
/// </summary>
public class GetChatsRequest
{
    /// <summary>
    /// По умолчанию: 50
    /// Количество запрашиваемых чатов
    /// </summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }

    /// <summary>
    /// Указатель на следующую страницу данных. 
    /// Для первой страницы передайте null
    /// </summary>
    [JsonPropertyName("marker")]
    public long? Marker { get; set; }
}
