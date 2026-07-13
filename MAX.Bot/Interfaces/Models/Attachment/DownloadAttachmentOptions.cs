using MAX.Bot.Interfaces.Models.Response;

namespace MAX.Bot.Interfaces.Models.Attachment;

/// <summary>
/// Параметры скачивания вложения. Повторные попытки наследуются из <see cref="AttachmentRetryOptions"/> (по умолчанию 4 попытки, пауза 2 секунды).
/// </summary>
public class DownloadAttachmentOptions : AttachmentRetryOptions
{
    /// <summary>
    /// Путь для сохранения файла на диск. Если не указан, содержимое возвращается в <see cref="AttachmentDownloadResult.Content"/>.
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// Качество видео при скачивании <see cref="Request.Message.Attachment.VideoAttachment"/>. По умолчанию — 480p.
    /// </summary>
    public VideoQuality VideoQuality { get; set; } = VideoQuality.Mp4_480;

    /// <summary>
    /// Расширение файла по умолчанию, если его не удалось определить из имени или URL.
    /// </summary>
    public string? DefaultExtension { get; set; }
}
