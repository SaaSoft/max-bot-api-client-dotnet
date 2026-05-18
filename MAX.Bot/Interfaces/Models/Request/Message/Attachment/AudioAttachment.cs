namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class AudioAttachment : Attachment
{
    public new Payloads.AudioPayload Payload { get; set; } = new();
}