using System.Text.Json.Serialization;
using UpdateTypeConstants = MAX.Bot.Interfaces.Models.UpdateTypes;
using static MAX.Bot.FrameworkSpecificMethods;

namespace MAX.Bot.Interfaces.Models.Request;

/// <summary>
/// Запрос на настройку подписки Webhook
/// </summary>
public record SubscriptionRequest
{
    /// <summary>
    /// URL HTTPS-endpoint вашего бота
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Список типов событий, которые хочет получать бот
    /// </summary>
    [JsonPropertyName("update_types")]
    public List<string>? UpdateTypes { get; set; }

    /// <summary>
    /// Секрет, который MAX будет отправлять в заголовке X-Max-Bot-Api-Secret
    /// </summary>
    [JsonPropertyName("secret")]
    public string? Secret { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Url))
            throw new ArgumentException("URL Webhook не должен быть пустым.", nameof(Url));

        if (!Uri.TryCreate(Url, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
            throw new ArgumentException("URL Webhook должен быть абсолютным HTTPS URL и начинаться с https://.", nameof(Url));

        if (!uri.IsDefaultPort && uri.Port != 443)
            throw new ArgumentException("URL Webhook должен использовать HTTPS-порт 443.", nameof(Url));

        if (UpdateTypes is { Count: 0 })
            throw new ArgumentException("Список типов событий не должен быть пустым. Передайте null, чтобы не задавать фильтр.", nameof(UpdateTypes));

        if (UpdateTypes is not null)
        {
            var uniqueUpdateTypes = new HashSet<string>(StringComparer.Ordinal);

            foreach (var updateType in UpdateTypes)
            {
                if (string.IsNullOrWhiteSpace(updateType))
                    throw new ArgumentException("Тип события в update_types не должен быть пустым.", nameof(UpdateTypes));

                if (!IsSupportedUpdateType(updateType))
                    throw new ArgumentException($"Неподдерживаемый тип события: {updateType}.", nameof(UpdateTypes));

                if (!uniqueUpdateTypes.Add(updateType))
                    throw new ArgumentException($"Тип события {updateType} указан повторно.", nameof(UpdateTypes));
            }
        }

        if (Secret is not null)
            ValidateSecret(Secret);
    }

    private static void ValidateSecret(string secret)
    {
        if (secret.Length is < 5 or > 256)
            throw new ArgumentException("Secret должен содержать от 5 до 256 символов.", nameof(Secret));

        if (!secret.All(c => Char_IsAsciiLetterOrDigit(c) || c is '_' or '-'))
            throw new ArgumentException("Secret может содержать только символы A-Z, a-z, 0-9, '_' и '-'.", nameof(Secret));
    }

    private static bool IsSupportedUpdateType(string updateType)
    {
        return updateType is
            UpdateTypeConstants.BotAdded or
            UpdateTypeConstants.BotStarted or
            UpdateTypeConstants.BotStopped or
            UpdateTypeConstants.BotRemoved or
            UpdateTypeConstants.ChatTitleChanged or
            UpdateTypeConstants.DialogCleared or
            UpdateTypeConstants.DialogMuted or
            UpdateTypeConstants.DialogUnmuted or
            UpdateTypeConstants.DialogRemoved or
            UpdateTypeConstants.MessageCallback or
            UpdateTypeConstants.MessageCreated or
            UpdateTypeConstants.MessageEdited or
            UpdateTypeConstants.MessageRemoved or
            UpdateTypeConstants.UserAdded or
            UpdateTypeConstants.UserRemoved;
    }
}
