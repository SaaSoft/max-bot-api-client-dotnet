namespace MAX.Bot.Interfaces.Models.Response;

/// <summary>
/// Ответ на загрузку файла
/// </summary>
public record UploadResponse
{
	/// <summary>
	/// Токены картинок
	/// </summary>
	public record UploadImageElementResponse
	{
		[JsonPropertyName("token")]
		public string Token { get; set; }
	}

	/// <summary>
	/// URL для загрузки файла
	/// </summary>
	[JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Видео- или аудио-токен для отправки сообщения
    /// </summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>
    /// Набор токенов для картинок
    /// </summary>
	[JsonPropertyName("photos")]
	public Dictionary<string, UploadImageElementResponse> Photos { get; set; }

	/// <summary>
	/// Результат загрузки видео или аудио
	/// </summary>
	[JsonPropertyName("retval")]
    public JsonElement? Retval { get; set; }
}
