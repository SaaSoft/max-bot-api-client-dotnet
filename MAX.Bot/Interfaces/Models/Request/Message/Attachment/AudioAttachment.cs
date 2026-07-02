namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class AudioAttachment : Attachment<Payloads.AudioPayload>
{
    public override Payloads.AudioPayload Payload { get; set; } = new();

    public override string Type => AttachmentTypes.Audio;
}