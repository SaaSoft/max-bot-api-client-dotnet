using System.Text.Json;
using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models.Response;

/// <summary>
/// Подробная информация о прикреплённом видео
/// </summary>
public record VideoInfoResponse
{
    /// <summary>
    /// Токен видео-вложения
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// URL-ы для скачивания или воспроизведения видео. Может быть null, если видео недоступно
    /// </summary>
    [JsonPropertyName("urls")]
    public JsonElement? Urls { get; set; }

    /// <summary>
    /// Миниатюра видео
    /// </summary>
    [JsonPropertyName("thumbnail")]
    public JsonElement? Thumbnail { get; set; }

    /// <summary>
    /// Ширина видео
    /// </summary>
    [JsonPropertyName("width")]
    public long Width { get; set; }

    /// <summary>
    /// Высота видео
    /// </summary>
    [JsonPropertyName("height")]
    public long Height { get; set; }

    /// <summary>
    /// Длина видео в секундах
    /// </summary>
    [JsonPropertyName("duration")]
    public long Duration { get; set; }

    /// <summary>
    /// Возвращает предпочтительный URL для скачивания видео.
    /// </summary>
    public string? TryGetDownloadUrl() => TryGetDownloadUrl(Urls);

    internal static string? TryGetDownloadUrl(JsonElement? urls)
    {
        if (urls is not { } element || element.ValueKind != JsonValueKind.Object)
            return null;

        if (element.TryGetProperty("mp4_480", out var mp4480) && mp4480.ValueKind == JsonValueKind.String)
            return mp4480.GetString();

        foreach (var property in element.EnumerateObject())
        {
            if (property.Value.ValueKind == JsonValueKind.String)
                return property.Value.GetString();
        }

        return null;
    }
}
