using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models.Request;

/// <summary>
/// Запрос на редактирование команд бота.
/// Чтобы удалить все команды, передайте пустой список.
/// </summary>
public record UpdateBotCommandsRequest
{
    private const int MaxCommands = 32;

    /// <summary>
    /// Команды, которые поддерживает бот. Не больше 32 элементов
    /// </summary>
    [JsonPropertyName("commands")]
    public List<BotCommand> Commands { get; set; } = new();

    /// <summary>
    /// Проверить параметры запроса
    /// </summary>
    public void Validate()
    {
        if (Commands.Count > MaxCommands)
            throw new ArgumentException($"Список команд не должен содержать больше {MaxCommands} элементов.", nameof(Commands));

        var names = new HashSet<string>(StringComparer.Ordinal);

        foreach (var command in Commands)
        {
            command.Validate();

            if (!names.Add(command.Name))
                throw new ArgumentException($"Команда {command.Name} указана повторно.", nameof(Commands));
        }
    }
}
