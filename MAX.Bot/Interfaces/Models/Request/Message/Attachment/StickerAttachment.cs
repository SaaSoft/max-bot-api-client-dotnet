using MAX.Bot.Interfaces.JsonConverters;

namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class StickerAttachment : Attachment
{
    public Payloads.StickerPayload Payload { get; set; } = new();
    public override string Type => AttachmentTypes.Sticker;
}