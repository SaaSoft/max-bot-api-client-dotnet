namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class ImageAttachment : Attachment<Payloads.ImagePayload>
{
    public override Payloads.ImagePayload Payload { get; set; } = new();
    public override string Type => AttachmentTypes.Image;
}