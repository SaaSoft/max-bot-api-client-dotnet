using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces;

/// <summary>
/// Стандартный ответ от АПИ
/// </summary>
public class BaseResponse
{
    /// <summary>
    /// true, если запрос был успешным, false в противном случае
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Объяснительное сообщение, если результат не был успешным
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}