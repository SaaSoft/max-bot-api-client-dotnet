using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces;

/// <summary>
/// Модель запроса на добавление пользователя в чат
/// </summary>
public class AddChatMemberRequest
{
    /// <summary>
    /// ID чата
    /// </summary>
    [JsonPropertyName("chatId")]
    public long ChatId { get; set; }

    /// <summary>
    /// Массив ID пользователей для добавления в чат
    /// </summary>
    [JsonPropertyName("user_ids")]
    public long[] UserIds { get; set; }
}
