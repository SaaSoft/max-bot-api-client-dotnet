namespace MAX.Bot.Interfaces.Models.Response;

/// <summary>
/// Качество видео из объекта <c>urls</c> ответа GET /videos/{videoToken}.
/// </summary>
public enum VideoQuality
{
    /// <summary>1080p MP4.</summary>
    Mp4_1080,

    /// <summary>720p MP4.</summary>
    Mp4_720,

    /// <summary>480p MP4. Используется по умолчанию.</summary>
    Mp4_480,

    /// <summary>360p MP4.</summary>
    Mp4_360,

    /// <summary>240p MP4.</summary>
    Mp4_240,

    /// <summary>144p MP4.</summary>
    Mp4_144,

    /// <summary>HLS-поток.</summary>
    Hls,
}
