using MAX.Bot.Interfaces.JsonConverters;

namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class LocationAttachment : Attachment
{
    public Payloads.LocationPayload Payload { get; set; } = new();
    public override string Type => AttachmentTypes.Location;
}