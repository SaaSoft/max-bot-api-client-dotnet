namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class StickerAttachment : Attachment<Payloads.StickerPayload>
{
    public override Payloads.StickerPayload Payload { get; set; } = new();
    public override string Type => AttachmentTypes.Sticker;
}