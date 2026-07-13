namespace MAX.Bot.Interfaces.Models.Attachment;

/// <summary>
/// Результат скачивания вложения.
/// </summary>
public class AttachmentDownloadResult
{
    /// <summary>
    /// Содержимое файла. Заполняется, если <see cref="DownloadAttachmentOptions.FilePath"/> не указан.
    /// </summary>
    public byte[]? Content { get; init; }

    /// <summary>
    /// Путь к сохранённому файлу. Заполняется, если указан <see cref="DownloadAttachmentOptions.FilePath"/>.
    /// </summary>
    public string? SavedFilePath { get; init; }

    /// <summary>
    /// Имя файла с расширением.
    /// </summary>
    public string? FileName { get; init; }
}
