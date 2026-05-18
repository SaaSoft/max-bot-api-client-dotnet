namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment.Payloads;

public sealed class LocationPayload : AttachmentPayload
{
    /// <summary>
    /// Широта
    /// </summary>
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    /// <summary>
    /// Долгота
    /// </summary>
    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
}