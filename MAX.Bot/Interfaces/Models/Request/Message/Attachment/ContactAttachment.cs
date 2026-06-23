using MAX.Bot.Interfaces.JsonConverters;

namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class ContactAttachment : Attachment<Payloads.ContactPayload>
{
    public override Payloads.ContactPayload Payload { get; set; } = new();
    public override string Type => AttachmentTypes.Contact;
}