namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class InlineKeyboardAttachment : Attachment
{
    public Payloads.InlineKeyboardPayload Payload { get; set; } = new();
    public override string Type => AttachmentTypes.InlineKeyboard;
}