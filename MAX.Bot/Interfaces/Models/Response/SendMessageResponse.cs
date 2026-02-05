using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces;

/// <summary>
/// Модель ответа от АПИ на отправку сообщения
/// </summary>
public record SendMessageResponse
{
    /// <summary>
    /// Сообщение в чате
    /// </summary>
    [JsonPropertyName("message")]
    public Message? Message { get; set; }
}

/// <summary>
/// Модель сообщения
/// </summary>
public record Message
{
    /// <summary>
    /// Сообщение в чате
    /// </summary>
    [JsonPropertyName("sender")]
    public User? Sender { get; set; }

    /// <summary>
    /// Содержимое сообщения. Текст + вложения. Может быть null, 
    /// если сообщение содержит только пересланное сообщение
    /// </summary>
    [JsonPropertyName("body")]
    public MessageBody? Body { get; set; }
}

/// <summary>
/// Модель тела сообщения
/// </summary>
public record MessageBody
{
    /// <summary>
    /// Уникальный ID сообщения
    /// </summary>
    [JsonPropertyName("mid")]
    public string? Mid { get; set; }

    /// <summary>
    /// Новый текст сообщения
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}