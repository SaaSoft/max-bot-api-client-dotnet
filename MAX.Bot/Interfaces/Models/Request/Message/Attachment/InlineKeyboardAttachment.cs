namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class InlineKeyboardAttachment : Attachment<Payloads.InlineKeyboardPayload>
{
    public override Payloads.InlineKeyboardPayload Payload { get; set; } = new();
    public override string Type => AttachmentTypes.InlineKeyboard;
}