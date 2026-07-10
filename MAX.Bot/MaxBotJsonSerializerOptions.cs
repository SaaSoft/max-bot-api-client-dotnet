namespace MAX.Bot;

internal static class MaxBotJsonSerializerOptions
{
    public static JsonSerializerOptions Deserialize { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowOutOfOrderMetadataProperties = true,
    };

    public static JsonSerializerOptions Serialize { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        AllowOutOfOrderMetadataProperties = true,
    };
}
