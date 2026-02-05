using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces;

/// <summary>
/// Модель запроса на получение обновлений
/// </summary>
public class GetUpdatesRequest
{
    /// <summary>
    /// Максимальное количество обновлений для получения (1-1000)
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// Тайм-аут в секундах для долгого опроса (0-90)
    /// </summary>
    [JsonPropertyName("timeout")]
    public int? Timeout { get; set; }

    /// <summary>
    /// Указатель на следующее ожидаемое обновление
    /// </summary>
    [JsonPropertyName("marker")]
    public long? Marker { get; set; }

    /// <summary>
    /// Список типов обновлений для фильтрации
    /// </summary>
    [JsonPropertyName("types")]
    public List<string>? Types { get; set; }
}