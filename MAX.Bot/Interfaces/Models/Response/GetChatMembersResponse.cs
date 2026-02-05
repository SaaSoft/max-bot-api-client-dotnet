using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces;

/// <summary>
/// Модель ответа от АПИ на получение участникова чата
/// </summary>
public record GetChatMembersResponse
{
    /// <summary>
    /// Список участников чата с информацией о времени последней активности
    /// </summary>
    [JsonPropertyName("members")]
    public ChatMember[]? Members { get; set; }

    /// <summary>
    /// Указатель на следующую страницу данных
    /// </summary>
    [JsonPropertyName("marker")]
    public long? Marker { get; set; }
}
