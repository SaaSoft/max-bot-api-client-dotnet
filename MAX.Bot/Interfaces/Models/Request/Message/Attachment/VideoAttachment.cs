namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class VideoAttachment : Attachment<Payloads.VideoPayload>
{
    public override Payloads.VideoPayload Payload { get; set; } = new();
    public override string Type => AttachmentTypes.Video;
}