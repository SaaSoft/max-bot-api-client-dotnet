namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment.Payloads;

public sealed class StickerPayload : AttachmentPayload
{
    /// <summary>
    /// Код стикера
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; }
}