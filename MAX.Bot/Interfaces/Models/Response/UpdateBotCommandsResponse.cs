using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models.Response;

/// <summary>
/// Ответ на редактирование команд бота
/// </summary>
public record UpdateBotCommandsResponse
{
    /// <summary>
    /// Команды, которые поддерживает бот
    /// </summary>
    [JsonPropertyName("commands")]
    public List<BotCommand>? Commands { get; set; }
}
