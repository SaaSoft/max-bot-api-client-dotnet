using MAX.Bot.Interfaces.JsonConverters;

namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class FileAttachment : Attachment<Payloads.FilePayload>
{
    public override Payloads.FilePayload Payload { get; set; } = new();
    public override string Type => AttachmentTypes.File;
}