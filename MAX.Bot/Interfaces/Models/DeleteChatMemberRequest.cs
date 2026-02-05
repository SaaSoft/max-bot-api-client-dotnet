using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces;

/// <summary>
/// Модель запроса на удаление пользователя из чата
/// </summary>
public class DeleteChatMemberRequest
{
    /// <summary>
    /// ID чата
    /// </summary>
    [JsonPropertyName("chatId")]
    public long ChatId { get; set; }

    /// <summary>
    /// ID пользователя, которого нужно удалить из чата
    /// </summary>
    [JsonPropertyName("user_id")]
    public long UserId { get; set; }

    /// <summary>
    /// Если установлено в true, пользователь будет заблокирован в чате
    /// Применяется только для чатов с публичной или приватной ссылкой
    /// Игнорируется в остальных случаях
    /// </summary>
    [JsonPropertyName("block")]
    public bool? Block { get; set; }
}
