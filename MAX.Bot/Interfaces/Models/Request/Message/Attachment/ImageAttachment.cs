using MAX.Bot.Interfaces.JsonConverters;

namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class ImageAttachment : Attachment
{
    public Payloads.ImagePayload Payload { get; set; } = new();
    public override string Type => AttachmentTypes.Image;
}