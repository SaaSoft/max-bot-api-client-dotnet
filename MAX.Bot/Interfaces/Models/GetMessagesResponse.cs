using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces;

/// <summary>
/// Модель ответа от АПИ на получение сообщений
/// </summary>
public class GetMessagesResponse
{
    /// <summary>
    /// Массив сообщений
    /// </summary>
    [JsonPropertyName("messages")]
    public Message[]? Messages { get; set; }
}