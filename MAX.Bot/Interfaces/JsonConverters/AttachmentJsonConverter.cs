using MAX.Bot.Interfaces.Models.Request.Message.Attachment;

namespace MAX.Bot.Interfaces.JsonConverters;

public sealed class AttachmentJsonConverter : JsonConverter<Attachment>
{
    public override Attachment? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        var type = GetRequiredString(root, "type");

        return type switch
        {
            AttachmentTypes.Contact => Deserialize<ContactAttachment>(root, options),
            AttachmentTypes.Image => Deserialize<ImageAttachment>(root, options),
            AttachmentTypes.Video => Deserialize<VideoAttachment>(root, options),
            AttachmentTypes.Audio => Deserialize<AudioAttachment>(root, options),
            AttachmentTypes.File => Deserialize<FileAttachment>(root, options),
            AttachmentTypes.Sticker => Deserialize<StickerAttachment>(root, options),
            AttachmentTypes.Location => Deserialize<LocationAttachment>(root, options),
            // TODO: дописать конвертер для кнопок: AttachmentTypes.InlineKeyboard => Deserialize<InlineKeyboardAttachment>(root, options),
            // TODO: дописать ShareAttachment: AttachmentTypes.Share => ... 
            // а пока просто не вернем вложение вместо ошибки
            _ => null //throw new JsonException($"Unknown attachment type: {type}")
        };
    }

    private static T Deserialize<T>(JsonElement root, JsonSerializerOptions options) where T : Attachment
    {
        return JsonSerializer.Deserialize<T>(root.GetRawText(), options)
            ?? throw new JsonException($"Failed to deserialize {typeof(T).Name}");
    }

    private static string GetRequiredString(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var element) || element.ValueKind != JsonValueKind.String)
            throw new JsonException($"Missing or invalid '{propertyName}' property");

        return element.GetString()!;
    }

    public override void Write(Utf8JsonWriter writer, Attachment value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}