namespace MAX.Bot.Interfaces.Models.Request.Message.Attachment.Payloads;

public sealed class InlineKeyboardPayload : AttachmentPayload
{
    [JsonPropertyName("buttons")]
    public List<List<Button>> Buttons { get; set; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(CallbackButton), "callback")]
[JsonDerivedType(typeof(LinkButton), "link")]
[JsonDerivedType(typeof(RequestGeoButton), "request_geo_location")]
[JsonDerivedType(typeof(RequestContactButton), "request_contact")]
[JsonDerivedType(typeof(OpenAppButton), "open_app")]
[JsonDerivedType(typeof(MessageButton), "message")]
public abstract class Button{}

public sealed class CallbackButton : Button
{
    /// <summary>
    /// от 1 до 128 символовВидимый текст кнопки.
    /// Чтобы он отображался полностью, рекомендуем не превышать заданное количество символов в зависимости от размещения текста: 20 символов — при 1 кнопке в ряду, 10 — при 2, 5 — при 3, 3 — при 4
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }

    /// <summary>
    /// до 1024 символов
    /// Токен кнопки
    /// </summary>
    [JsonPropertyName("payload")]
    public string Payload { get; set; }
}

public sealed class LinkButton : Button
{
    /// <summary>
    /// от 1 до 128 символовВидимый текст кнопки.
    /// Чтобы он отображался полностью, рекомендуем не превышать заданное количество символов в зависимости от размещения текста: 20 символов — при 1 кнопке в ряду, 10 — при 2, 5 — при 3, 3 — при 4
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }

    /// <summary>
    /// до 2048 символов
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }
}

public sealed class RequestGeoButton : Button
{
    /// <summary>
    /// от 1 до 128 символовВидимый текст кнопки.
    /// Чтобы он отображался полностью, рекомендуем не превышать заданное количество символов в зависимости от размещения текста: 20 символов — при 1 кнопке в ряду, 10 — при 2, 5 — при 3, 3 — при 4
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }

    /// <summary>
    /// Если true, отправляет местоположение без запроса подтверждения пользователя
    /// </summary>
    [JsonPropertyName("quick")]
    public bool Quick { get; set; }
}

public sealed class RequestContactButton : Button
{
    /// <summary>
    /// от 1 до 128 символовВидимый текст кнопки.
    /// Чтобы он отображался полностью, рекомендуем не превышать заданное количество символов в зависимости от размещения текста: 20 символов — при 1 кнопке в ряду, 10 — при 2, 5 — при 3, 3 — при 4
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }
}

public sealed class OpenAppButton : Button
{
    /// <summary>
    /// от 1 до 128 символовВидимый текст кнопки.
    /// Чтобы он отображался полностью, рекомендуем не превышать заданное количество символов в зависимости от размещения текста: 20 символов — при 1 кнопке в ряду, 10 — при 2, 5 — при 3, 3 — при 4
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }

    /// <summary>
    /// Публичное имя (username) бота или ссылка на него, чьё мини-приложение надо запустить
    /// </summary>
    [JsonPropertyName("web_app")]
    public string? WebApp { get; set; }

    /// <summary>
    /// Идентификатор бота, чьё мини-приложение надо запустить
    /// </summary>
    [JsonPropertyName("contact_id")]
    public long? ContactId { get; set; }

    /// <summary>
    /// Параметр запуска, который будет передан в initData мини-приложения
    /// </summary>
    [JsonPropertyName("payload")]
    public string? Payload { get; set; }
}

public sealed class MessageButton : Button
{
    /// <summary>
    /// от 1 до 128 символовВидимый текст кнопки.
    /// Чтобы он отображался полностью, рекомендуем не превышать заданное количество символов в зависимости от размещения текста: 20 символов — при 1 кнопке в ряду, 10 — при 2, 5 — при 3, 3 — при 4
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }
}