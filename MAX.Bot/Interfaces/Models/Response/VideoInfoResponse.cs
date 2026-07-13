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
    public VideoUrls? Urls { get; set; }

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
    /// Возвращает URL для скачивания видео в выбранном качестве. По умолчанию — <see cref="VideoQuality.Mp4_480"/>.
    /// </summary>
    public string? TryGetDownloadUrl(VideoQuality quality = VideoQuality.Mp4_480) =>
        Urls?.TryGetUrl(quality);
}
