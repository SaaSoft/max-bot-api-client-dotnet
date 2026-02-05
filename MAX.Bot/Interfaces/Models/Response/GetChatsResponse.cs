using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces;

/// <summary>
/// Модель ответа от АПИ на получение чатов
/// </summary>
public record GetChatsResponse
{
    /// <summary>
    /// Список запрашиваемых чатов
    /// </summary>
    [JsonPropertyName("chats")]
    public Chat[]? Chats { get; set; }

    /// <summary>
    /// Указатель на следующую страницу запрашиваемых чатов
    /// </summary>
    [JsonPropertyName("marker")]
    public long? Marker { get; set; }
}
