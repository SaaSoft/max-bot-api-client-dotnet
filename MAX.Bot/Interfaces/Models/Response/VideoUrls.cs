using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models.Response;

/// <summary>
/// URL-адреса видео в разных разрешениях (объект <c>urls</c> из GET /videos/{videoToken}).
/// </summary>
public record VideoUrls
{
    [JsonPropertyName("mp4_1080")]
    public string? Mp4_1080 { get; init; }

    [JsonPropertyName("mp4_720")]
    public string? Mp4_720 { get; init; }

    [JsonPropertyName("mp4_480")]
    public string? Mp4_480 { get; init; }

    [JsonPropertyName("mp4_360")]
    public string? Mp4_360 { get; init; }

    [JsonPropertyName("mp4_240")]
    public string? Mp4_240 { get; init; }

    [JsonPropertyName("mp4_144")]
    public string? Mp4_144 { get; init; }

    [JsonPropertyName("hls")]
    public string? Hls { get; init; }

    /// <summary>
    /// Возвращает URL выбранного качества. Если оно недоступно, пробует <see cref="VideoQuality.Mp4_480"/>, затем любой доступный URL.
    /// </summary>
    public string? TryGetUrl(VideoQuality quality = VideoQuality.Mp4_480)
    {
        var url = GetUrl(quality);
        if (!string.IsNullOrWhiteSpace(url))
            return url;

        if (quality != VideoQuality.Mp4_480 && !string.IsNullOrWhiteSpace(Mp4_480))
            return Mp4_480;

        return Mp4_1080
            ?? Mp4_720
            ?? Mp4_480
            ?? Mp4_360
            ?? Mp4_240
            ?? Mp4_144
            ?? Hls;
    }

    private string? GetUrl(VideoQuality quality) => quality switch
    {
        VideoQuality.Mp4_1080 => Mp4_1080,
        VideoQuality.Mp4_720 => Mp4_720,
        VideoQuality.Mp4_480 => Mp4_480,
        VideoQuality.Mp4_360 => Mp4_360,
        VideoQuality.Mp4_240 => Mp4_240,
        VideoQuality.Mp4_144 => Mp4_144,
        VideoQuality.Hls => Hls,
        _ => null,
    };
}
