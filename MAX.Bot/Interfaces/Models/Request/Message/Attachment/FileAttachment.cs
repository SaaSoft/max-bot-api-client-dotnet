namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment;

public sealed class FileAttachment : Attachment
{
    /// <summary>
    /// Имя файла
    /// </summary>
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }

    /// <summary>
    /// Размер файла в байтах
    /// </summary>
    [JsonPropertyName("size")]
    public long? Size { get; set; }

    public new Payloads.FilePayload Payload { get; set; } = new();
}