using System.Text.Json.Serialization;

/// <summary>
/// Формат сообщения
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MessageFormat
{
    /// <summary>
    /// Markdown разметка
    /// </summary>
    Markdown,

    /// <summary>
    /// HTML разметка
    /// </summary>
    Html
}