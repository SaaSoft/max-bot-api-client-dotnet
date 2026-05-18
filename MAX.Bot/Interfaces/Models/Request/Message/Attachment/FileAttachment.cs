namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class FileAttachment : Attachment
{
    public new Payloads.FilePayload Payload { get; set; } = new();
}