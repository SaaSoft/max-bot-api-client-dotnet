using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace MAX.Bot.Interfaces;

/// <summary>
/// Модель чата
/// </summary>
public record Chat
{
    /// <summary>
    /// ID чата
    /// </summary>
    [JsonPropertyName("chat_id")]
    public long ChatId { get; set; }

    /// <summary>
    /// Тип чата
    /// "chat" — Групповой чат
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Статус чата
    /// "active" — Бот является активным участником чата
    /// "removed" — Бот был удалён из чата
    /// "left" — Бот покинул чат
    /// "closed" — Чат был закрыт
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// Отображаемое название чата
    /// Может быть null для диалогов
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Иконка чата
    /// </summary>
    [JsonPropertyName("icon")]
    public Image? Icon { get; set; }

    /// <summary>
    /// Время последнего события в чате
    /// </summary>
    [JsonPropertyName("last_event_time")]
    public long LastEventTime { get; set; }

    /// <summary>
    /// Количество участников чата
    /// Для диалогов всегда 2
    /// </summary>
    [JsonPropertyName("participants_count")]
    public int ParticipantsCount { get; set; }

    /// <summary>
    /// ID владельца чата
    /// </summary>
    [JsonPropertyName("owner_id")]
    public long? OwnerId { get; set; }

    /// <summary>
    /// Участники чата с временем последней активности
    /// Может быть null, если запрашивается список чатов
    /// </summary>
    [JsonPropertyName("participants")]
    public object? Participants { get; set; }

    /// <summary>
    /// Доступен ли чат публично
    /// Для диалогов всегда false
    /// </summary>
    [JsonPropertyName("is_public")]
    public bool IsPublic { get; set; }

    /// <summary>
    /// Ссылка на чат
    /// </summary>
    [JsonPropertyName("link")]
    public string? Link { get; set; }

    /// <summary>
    /// Описание чата
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Данные о пользователе в диалоге
    /// Только для чатов типа "dialog"
    /// </summary>
    [JsonPropertyName("dialog_with_user")]
    public User? DialogWithUser { get; set; }

    /// <summary>
    /// ID сообщения, содержащего кнопку, через которую был инициирован чат
    /// </summary>
    [JsonPropertyName("chat_message_id")]
    public string? ChatMessageId { get; set; }

    /// <summary>
    /// Закреплённое сообщение в чате
    /// Возвращается только при запросе конкретного чата
    /// </summary>
    [JsonPropertyName("pinned_message")]
    public Message? PinnedMessage { get; set; }
}

/// <summary>
/// Статус чата
/// </summary>
public static class ChatStatus
{
    /// <summary>
    /// Бот является активным участником чата
    /// </summary>
    public const string Active = "active";

    // <summary>
    /// Бот был удалён из чата
    /// </summary>
    public const string Removed = "removed";

    /// <summary>
    /// Бот покинул чат
    /// </summary>
    public const string Left = "left";

    /// <summary>
    /// Чат был закрыт
    /// </summary>
    public const string Closed = "closed";
}