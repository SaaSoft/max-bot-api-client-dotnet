using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces;

/// <summary>
/// Модель ответа от АПИ на получение обновлений
/// </summary>
public record GetUpdatesResponse
{
    /// <summary>
    /// Страница обновлений
    /// </summary>
    [JsonPropertyName("updates")]
    public List<Update> Updates { get; set; } = new();

    /// <summary>
    /// Указатель на следующую страницу данных
    /// </summary>
    [JsonPropertyName("marker")]
    public long? Marker { get; set; }
}