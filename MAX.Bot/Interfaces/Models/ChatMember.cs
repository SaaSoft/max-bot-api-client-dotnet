using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces;

/// <summary>
/// Модель участника чата
/// </summary>
public record ChatMember
{
    /// <summary>
    /// Идентификатор пользователя или бота
    /// </summary>
    [JsonPropertyName("user_id")]
    public long UserId { get; set; }

    /// <summary>
    /// Отображаемое имя пользователя или бота
    /// </summary>
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Отображаемая фамилия пользователя. Для ботов это поле не возвращается
    /// </summary>
    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    /// <summary>
    /// Никнейм бота или уникальное публичное имя пользователя
    /// В случае с пользователем может быть null, если тот недоступен или имя не задано
    /// </summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    /// <summary>
    /// true, если это бот
    /// </summary>
    [JsonPropertyName("is_bot")]
    public bool IsBot { get; set; }

    /// <summary>
    /// Время последней активности пользователя или бота в MAX (Unix-время в миллисекундах)
    /// Если пользователь отключил в настройках профиля мессенджера MAX возможность видеть,
    /// что он в сети онлайн, поле может не возвращаться
    /// </summary>
    [JsonPropertyName("last_activity_time")]
    public long LastActivityTime { get; set; }

    /// <summary>
    /// Устаревшее поле, скоро будет удалено
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Описание пользователя или бота. В случае с пользователем может принимать значение null,
    /// если описание не заполнено. До 16000 символов
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// URL аватара пользователя или бота в уменьшенном размере
    /// </summary>
    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// URL аватара пользователя или бота в полном размере
    /// </summary>
    [JsonPropertyName("full_avatar_url")]
    public string? FullAvatarUrl { get; set; }

    /// <summary>
    /// Время последней активности пользователя в чате.
    /// Может быть устаревшим для суперчатов (равно времени вступления)
    /// </summary>
    [JsonPropertyName("last_access_time")]
    public long LastAccessTime { get; set; }

    /// <summary>
    /// Является ли пользователь владельцем чата
    /// </summary>
    [JsonPropertyName("is_owner")]
    public bool IsOwner { get; set; }

    /// <summary>
    /// Является ли пользователь администратором чата
    /// </summary>
    [JsonPropertyName("is_admin")]
    public bool IsAdmin { get; set; }

    /// <summary>
    /// Дата присоединения к чату в формате Unix time
    /// </summary>
    [JsonPropertyName("join_time")]
    public long JoinTime { get; set; }

    /// <summary>
    /// Перечень прав пользователя
    /// </summary>
    [JsonPropertyName("permissions")]
    public List<string>? Permissions { get; set; }

    /// <summary>
    /// Заголовок, который будет показан на клиенте
    /// Если пользователь администратор или владелец и ему не установлено это название,
    /// то поле не передаётся, клиенты на своей стороне подменят на "владелец" или "админ"
    /// </summary>
    [JsonPropertyName("alias")]
    public string? Alias { get; set; }
}

/// <summary>
/// Права администратора чата
/// </summary>
public static class ChatAdminPermission
{
    /// <summary>
    /// Читать все сообщения
    /// </summary>
    public const string ReadAllMessages = "read_all_messages";
    /// <summary>
    /// Добавлять/удалять участников
    /// </summary>
    public const string AddRemoveMembers = "add_remove_members";
    /// <summary>
    /// Добавлять администраторов
    /// </summary>
    public const string AddAdmins = "add_admins";
    /// <summary>
    /// Изменять информацию о чате
    /// </summary>
    public const string ChangeChatInfo = "change_chat_info";
    /// <summary>
    /// Закреплять сообщения
    /// </summary>
    public const string PinMessage = "pin_message";
    /// <summary>
    /// Писать сообщения
    /// </summary>
    public const string Write = "write";
    /// <summary>
    /// Изменять ссылку на чат
    /// </summary>
    public const string EditLink = "edit_link";
    /// <summary>
    /// Может звонить
    /// </summary>
    public const string CanCall = "can_call";
    /// <summary>
    /// Отправлять/Редактировать/Удалять сообщения
    /// </summary>
    public const string PostEditDeleteMessage = "post_edit_delete_message";
    /// <summary>
    /// Редактировать сообщения
    /// </summary>
    public const string EditMessage = "edit_message";
    /// <summary>
    /// Удалять сообщения
    /// </summary>
    public const string DeleteMessage = "delete_message";
}
