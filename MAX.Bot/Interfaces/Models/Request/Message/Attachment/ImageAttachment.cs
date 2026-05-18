namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class ImageAttachment : Attachment
{
    public new Payloads.ImagePayload Payload { get; set; } = new();
}