using MAX.Bot.Interfaces.JsonConverters;

namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class VideoAttachment : Attachment
{
    public Payloads.VideoPayload Payload { get; set; } = new();
    public override string Type => AttachmentTypes.Video;
}