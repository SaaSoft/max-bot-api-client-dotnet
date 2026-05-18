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
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(InlineKeyboardPayload), "inline_keyboard")]
[JsonDerivedType(typeof(VideoPayload), "video")]
[JsonDerivedType(typeof(ImagePayload), "image")]
[JsonDerivedType(typeof(AudioPayload), "audio")]
[JsonDerivedType(typeof(FilePayload), "file")]
[JsonDerivedType(typeof(StickerPayload), "sticker")]
[JsonDerivedType(typeof(ContactPayload), "contact")]
[JsonDerivedType(typeof(LocationPayload), "location")]
public abstract class AttachmentPayload { }
