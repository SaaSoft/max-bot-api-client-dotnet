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
    public virtual Task<string?> GetDownloadUrlAsync(
        IMaxBotClient client,
        CancellationToken cancellationToken = default)
    {
        return this switch
        {
            ImageAttachment image => Task.FromResult<string?>(image.Payload.Url),
            FileAttachment file => Task.FromResult(file.Payload.Url),
            VideoAttachment video => GetVideoDownloadUrlAsync(client, video, cancellationToken),
            _ => Task.FromResult<string?>(null),

        };
    }

    /// <summary>
    /// Возвращает предпочтительное имя файла для скачивания вложения.
    /// </summary>
    public virtual string? GetFileName() =>
        this is FileAttachment file ? file.Filename : null;

    private static async Task<string?> GetVideoDownloadUrlAsync(
        IMaxBotClient client,
        VideoAttachment video,
        CancellationToken cancellationToken)
    {
        var videoInfo = await client.GetVideoAsync(video.Payload.Token, cancellationToken);
        return videoInfo.TryGetDownloadUrl();
    }

}

public abstract class AttachmentPayload { }
