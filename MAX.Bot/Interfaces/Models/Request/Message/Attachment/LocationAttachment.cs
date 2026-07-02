namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class LocationAttachment : Attachment<Payloads.LocationPayload>
{
    public override Payloads.LocationPayload Payload { get; set; } = new();
    public override string Type => AttachmentTypes.Location;
}