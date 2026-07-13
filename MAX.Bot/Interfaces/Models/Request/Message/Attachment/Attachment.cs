using MAX.Bot.Interfaces;
using MAX.Bot.Interfaces.Models.Request.Message.Attachment.Payloads;

namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(InlineKeyboardAttachment), "inline_keyboard")]
[JsonDerivedType(typeof(VideoAttachment), "video")]
[JsonDerivedType(typeof(ImageAttachment), "image")]
[JsonDerivedType(typeof(AudioAttachment), "audio")]
[JsonDerivedType(typeof(FileAttachment), "file")]
[JsonDerivedType(typeof(StickerAttachment), "sticker")]
[JsonDerivedType(typeof(ContactAttachment), "contact")]
[JsonDerivedType(typeof(LocationAttachment), "location")]
public abstract class Attachment
{
    [JsonPropertyName("payload")]
    public AttachmentPayload? Payload { get; set; }

    /// <summary>
    /// Возвращает URL для скачивания вложения, если он доступен напрямую или через API.
    /// </summary>
    public virtual async Task<string?> GetDownloadUrlAsync(
        IMaxBotClient client,
        CancellationToken cancellationToken = default)
    {
        switch (this)
        {
            case ImageAttachment image:
                return image.Payload.Url;
            case FileAttachment file:
                return file.Payload.Url;
            case VideoAttachment video:
                return (await client.GetVideoAsync(video.Payload.Token, cancellationToken)).TryGetDownloadUrl();
            default:
                return null;
        }
    }

    /// <summary>
    /// Возвращает предпочтительное имя файла для скачивания вложения.
    /// </summary>
    public virtual string? GetFileName() =>
        this is FileAttachment file ? file.Filename : null;
}

public abstract class AttachmentPayload { }
