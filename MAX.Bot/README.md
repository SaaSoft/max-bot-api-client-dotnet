# MAX Bot Client для .NET

.NET клиент для работы с API мессенджера MAX. Библиотека предоставляет полный набор методов для взаимодействия с ботами, включая отправку сообщений, управление чатами и обработку событий в реальном времени.

## Поддерживаемые платформы

- **.NET 8.0 / 9.0 / 10.0**
- **.NET Framework 4.6.2** (для совместимости с устаревшими системами)

## Особенности

- ✅ **Долгосрочный polling** — обработка событий в реальном времени
- ✅ **Dependency Injection** — готовая интеграция с ASP.NET Core
- ✅ **Гибкая конфигурация** — несколько способов создания клиента
- ✅ **Кросс‑платформенность** — поддержка современных .NET и классического .NET Framework

## Установка

### Через NuGet Package Manager
```bash
Install-Package SaaSoft.MAX.Bot
```

### Через .NET CLI
```bash
dotnet add package SaaSoft.MAX.Bot
```

### Через PackageReference
```xml
<PackageReference Include="SaaSoft.MAX.Bot" Version="1.2.2" />
```

## Быстрый старт

### 1. Получение токена бота

Перед началом работы получите токен бота в [Личном кабинете разработчика MAX](https://dev.max.ru).

### 2. Базовое использование

```csharp
using MAX.Bot;
using MAX.Bot.Interfaces;
using MAX.Bot.Interfaces.Models;

// Создание клиента с токеном
var botClient = new MaxBotClient("YOUR_BOT_TOKEN");

// Получение информации о боте
var botInfo = await botClient.GetMeAsync();
Console.WriteLine($"Бот: {botInfo.FirstName} (ID: {botInfo.Id})");

// Отправка сообщения
await botClient.SendMessageAsync(new SendMessageRequest
{
    ChatId = 123456789,
    Text = "Привет от бота! 👋",
    Format = MessageFormat.Markdown,
});

// Получение обновлений
var _ = botClient.PollUpdatesWithCallback(
    async (update, client) =>
    {
        if (update is MessageCreatedUpdate messageCreated)
        {
            Console.WriteLine($"Сообщение: {messageCreated.Message?.Body?.Text}");

            await client.SendMessageAsync(new SendMessageRequest
            {
                Text = messageCreated.Message?.Body?.Text,
                ChatId = -70581633278133,
            });
        }
    },
    limit: 100,
    timeout: 90,
    types: new List<string> { UpdateTypes.MessageCreated }
);
```

## Способы создания клиента

### 1. Простой конструктор (рекомендуется для консольных приложений)

```csharp
using MAX.Bot.Interfaces.Models.Attachment;

// С токеном и таймаутом по умолчанию (30 секунд)
var client = new MaxBotClient("your_token_here");

// С кастомным таймаутом и retry для вложений
var client = new MaxBotClient("your_token_here", timeoutSeconds: 60,
    attachmentRetryOptions: new AttachmentRetryOptions { MaxAttempts = 4, RetryDelay = TimeSpan.FromSeconds(2) });
```

### 2. Dependency Injection (рекомендуется для ASP.NET Core)

```csharp
// В Program.cs
builder.Services.AddMaxBotClient(builder.Configuration["MaxBot:Token"]);

// В классе сервиса
public class BotService
{
    private readonly IMaxBotClient _botClient;
    
    public BotService(IMaxBotClient botClient)
    {
        _botClient = botClient;
    }
    
    public async Task SendWelcomeMessage(long chatId)
    {
        await _botClient.SendMessageAsync(new SendMessageRequest
        {
            ChatId = chatId,
            Text = "Добро пожаловать!"
        });
    }
}
```

## Скачивание файлов

```csharp
using MAX.Bot.Interfaces.Models.Attachment;
using MAX.Bot.Interfaces.Models.Response;

var message = await botClient.GetMessageByIdAsync("message_id");

foreach (var attachment in message.Body?.Attachments ?? [])
{
    try
    {
        var result = await botClient.DownloadAttachmentAsync(attachment, new DownloadAttachmentOptions
        {
            FilePath = Path.Combine("downloads", message.Body!.Mid!),
            VideoQuality = VideoQuality.Mp4_720,
        });

        Console.WriteLine($"Сохранено: {result.SavedFilePath}");
    }
    catch (NotSupportedException)
    {
        // sticker, keyboard и др. — скачивание не поддерживается
    }
}
```

Без `FilePath` содержимое возвращается в `result.Content`:

```csharp
var result = await botClient.DownloadAttachmentAsync(attachment, new DownloadAttachmentOptions
{
    VideoQuality = VideoQuality.Mp4_720,
});

var content = result.Content!;
Console.WriteLine($"Получено {content.Length} байт");
```
