using MAX.Bot.Interfaces.Models.Request.Message.Attachment.Payloads;

namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public abstract class Attachment
{
    [JsonPropertyName("type")]
    public abstract string Type { get; }
}

public abstract class Attachment<T> : Attachment where T : AttachmentPayload
{
    [JsonPropertyName("payload")]
    public abstract T Payload { get; set; }
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

/// <summary>
/// Типы вложений
/// </summary>
public static class AttachmentTypes
{
    public const string InlineKeyboard = "inline_keyboard";
    public const string Video = "video";
    public const string Image = "image";
    public const string Audio = "audio";
    public const string File = "file";
    public const string Sticker = "sticker";
    public const string Contact = "contact";
    public const string Location = "location";
    public const string Share = "share";
}
