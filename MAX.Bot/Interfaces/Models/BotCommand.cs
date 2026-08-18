using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models;

/// <summary>
/// Команда бота
/// </summary>
public record BotCommand
{
    /// <summary>
    /// Название команды. От 1 до 64 символов
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Описание команды. От 1 до 128 символов
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Проверить параметры команды
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name) || Name.Length > 64)
            throw new ArgumentException("Название команды должно быть от 1 до 64 символов.", nameof(Name));

        if (Description is not null && (string.IsNullOrWhiteSpace(Description) || Description.Length > 128))
            throw new ArgumentException("Описание команды должно быть от 1 до 128 символов.", nameof(Description));
    }
}
