using MAX.Bot.Interfaces.JsonConverters;

namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class AudioAttachment : Attachment
{
    public Payloads.AudioPayload Payload { get; set; } = new();

    public override string Type => AttachmentTypes.Audio;
}