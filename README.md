# MAX Bot Client для .NET

.NET клиент для работы с API мессенджера MAX. Библиотека предоставляет полный набор методов для взаимодействия с ботами, включая отправку сообщений, управление чатами и обработку событий в реальном времени.

## Особенности

- ✅ **Долгосрочный polling** — обработка событий в реальном времени
- ✅ **Dependency Injection** — готовая интеграция с ASP.NET Core
- ✅ **Вложения** — скачивание image/video/file, выбор качества видео
- ✅ **Повторы** — автоматические retry при обработке вложений API

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
<PackageReference Include="SaaSoft.MAX.Bot" Version="1.2.0" />
```

### Сертификаты Минцифры

API MAX работает на `https://platform-api2.max.ru` и использует TLS-сертификаты Национального удостоверяющего центра Минцифры (Russian Trusted CA). Этого корневого сертификата нет в стандартном хранилище доверенных CA Windows и .NET, поэтому без его установки запросы могут завершаться ошибкой SSL/TLS.

Добавьте сертификаты Минцифры в доверенные на машине, где запускается бот (Windows, Linux или сервер). Инструкция и файлы сертификатов — на портале Госуслуг: [gosuslugi.ru/crt](https://www.gosuslugi.ru/crt).

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

## API методов

Документация API методов для реализации в библиотеке.

### Легенда статусов

- ✅ — Реализовано полностью
- 🚧 — Реализовано частично
- ❌ — Не реализовано

### Методы API

#### 🤖 Bots

| Метод | Описание | Ссылка | Статус |
|-------|----------|--------|--------|
| GET | Получение информации о боте | `GET/me` | ✅ |

#### ✉️ Messages

| Метод | Описание | Ссылка | Статус |
|-------|----------|--------|--------|
| GET | Получение сообщений | `GET/messages` | ✅ |
| POST | Отправить сообщение | `POST/messages` | ✅ |
| PUT | Редактировать сообщение | `PUT/messages` | ✅ |
| DEL | Удалить сообщение | `DELETE/messages` | ✅ |
| GET | Получить сообщение | `GET/messages/-messageId-` | ✅ |
| GET | Получить информацию о видео | `GET/videos/-videoToken-` | ✅ |
| POST | Ответ на callback | `POST/answers` | ✅ |


#### 💬 Chats

| Метод | Описание | Ссылка | Статус |
|-------|----------|--------|--------|
| GET | Получение списка всех групповых чатов | `GET/chats` | ✅ |
| GET | Получение информации о групповом чате | `GET/chats/-chatId-` | ✅ |
| PATCH | Изменение информации о групповом чате | `PATCH/chats/-chatId-` | ✅ |
| DEL | Удаление группового чата | `DELETE/chats/-chatId-` | ✅ |
| POST | Отправка действия бота в групповой чат | `POST/chats/-chatId-/actions` | ✅ |
| GET | Получение закреплённого сообщения в групповом чате | `GET/chats/-chatId-/pin` | ✅ |
| PUT | Закрепление сообщения в групповом чате | `PUT/chats/-chatId-/pin` | ✅ |
| DEL | Удаление закреплённого сообщения в групповом чате | `DELETE/chats/-chatId-/pin` | ✅ |
| GET | Получение информации о членстве бота в групповом чате | `GET/chats/-chatId-/members/me` | ✅ |
| DEL | Удаление бота из группового чата | `DELETE/chats/-chatId-/members/me` | ✅ |
| GET | Получение списка администраторов группового чата | `GET/chats/-chatId-/members/admins` | ✅ |
| POST | Назначить администратора группового чата | `POST/chats/-chatId-/members/admins` | ✅ |
| DEL | Отменить права администратора в групповом чате | `DELETE/chats/-chatId-/members/admins/-userId-` | ✅ |
| GET | Получение участников группового чата | `GET/chats/-chatId-/members` | ✅ |
| POST | Добавление участников в групповой чат | `POST/chats/-chatId-/members` | ✅ |
| DEL | Удаление участника из группового чата | `DELETE/chats/-chatId-/members` | ✅ |

#### 📡 Subscriptions

| Метод | Описание | Ссылка | Статус |
|-------|----------|--------|--------|
| GET | Получение подписок | `GET/subscriptions` | ✅ |
| POST | Подписка на обновления | `POST/subscriptions` | ✅ |
| DEL | Отписка от обновлений | `DELETE/subscriptions` | ✅ |
| GET | Получение обновлений | `GET/updates` | ✅ |

#### 📁 Upload

| Метод | Описание | Ссылка | Статус |
|-------|----------|--------|--------|
| POST | Загрузка файлов | `POST/uploads` | ✅ |
