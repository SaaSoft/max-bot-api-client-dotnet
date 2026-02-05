using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces;

/// <summary>
/// Модель изображения
/// </summary>
public record Image
{
    /// <summary>
    /// URL изображения
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
