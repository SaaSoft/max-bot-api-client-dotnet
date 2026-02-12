using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models;

/// <summary>
/// Модель обновления
/// </summary>
public record Update
{
    /// <summary>
    /// Тип обновления (UpdateTypes)
    /// </summary>
    [JsonPropertyName("update_type")]
    public string UpdateType { get; set; } = string.Empty;

    /// <summary>
    /// Unix-время, когда произошло событие
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    /// <summary>
    /// Новое созданное сообщение
    /// </summary>
    [JsonPropertyName("message")]
    public Message? Message { get; set; }

    /// <summary>
    /// Текущий язык пользователя в формате IETF BCP 47. 
    /// Доступно только в диалогах
    /// </summary>
    [JsonPropertyName("user_locale")]
    public string? UserLocale { get; set; }

    /// <summary>
    /// Дополнительные данные из дип-линков, переданные при запуске бота
    /// </summary>
    [JsonPropertyName("payload")]
    public string? Payload { get; set; }

    /// <summary>
    /// Пользователь
    /// </summary>
    [JsonPropertyName("user")]
    public User? User { get; set; }
}

/// <summary>
/// Типы обновлений (событий) в системе
/// </summary>
public static class UpdateTypes
{
    /// <summary>
    /// Создание нового сообщения
    /// </summary>
    public const string MessageCreated = "message_created";

    /// <summary>
    /// Обратный вызов (callback) от кнопки сообщения
    /// </summary>
    public const string MessageCallback = "message_callback";

    /// <summary>
    /// Редактирование существующего сообщения
    /// </summary>
    public const string MessageEdited = "message_edited";

    /// <summary>
    /// Удаление сообщения
    /// </summary>
    public const string MessageDeleted = "message_deleted";

    /// <summary>
    /// Новый участник присоединился к чату
    /// </summary>
    public const string ChatMemberJoined = "chat_member_joined";

    /// <summary>
    /// Участник покинул чат
    /// </summary>
    public const string ChatMemberLeft = "chat_member_left";

    /// <summary>
    /// Изменение названия чата
    /// </summary>
    public const string ChatTitleChanged = "chat_title_changed";
}