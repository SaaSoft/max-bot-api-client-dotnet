namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class ContactAttachment : Attachment
{
    public new Payloads.ContactPayload Payload { get; set; } = new();
}