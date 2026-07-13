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
    /// Возвращает предпочтительное имя файла для скачивания вложения.
    /// </summary>
    public virtual string? GetFileName() =>
        this is FileAttachment file ? file.Filename : null;
}

public abstract class AttachmentPayload { }
