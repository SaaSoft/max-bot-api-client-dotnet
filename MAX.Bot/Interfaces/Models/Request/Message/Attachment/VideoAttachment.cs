namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class VideoAttachment : Attachment
{
    public new Payloads.VideoPayload Payload { get; set; } = new();
}