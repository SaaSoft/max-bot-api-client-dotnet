using MAX.Bot.Extensions;
using MAX.Bot.Interfaces;
using MAX.Bot.Interfaces.Models;
using MAX.Bot.Interfaces.Models.Attachment;
using MAX.Bot.Interfaces.Models.Request;
using MAX.Bot.Interfaces.Models.Request.Message;
using MAX.Bot.Interfaces.Models.Request.Message.Attachment;
using MAX.Bot.Interfaces.Models.Request.Message.Attachment.Payloads;
using MAX.Bot.Interfaces.Models.Response;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Attachment = MAX.Bot.Interfaces.Models.Request.Message.Attachment.Attachment;

const string C_BOT_API = "";
const long C_TEST_CHAT_ID = -70581633278133;
const long C_TEST_USER_ID = 168973682;
const string C_TEST_WEBHOOK_URL = "https://your-domain.com/webhook";

var enableDeleteChatTest = bool.TryParse(Environment.GetEnvironmentVariable("MAX_ENABLE_DELETE_CHAT_TEST"), out var parsedDeleteChatTest) && parsedDeleteChatTest;
var deleteTestChatId = long.TryParse(Environment.GetEnvironmentVariable("MAX_DELETE_TEST_CHAT_ID"), out var parsedDeleteChatId) ? parsedDeleteChatId : 0L;
var enableLeaveChatTest = bool.TryParse(Environment.GetEnvironmentVariable("MAX_ENABLE_LEAVE_CHAT_TEST"), out var parsedLeaveChatTest) && parsedLeaveChatTest;
var leaveTestChatId = long.TryParse(Environment.GetEnvironmentVariable("MAX_LEAVE_TEST_CHAT_ID"), out var parsedLeaveChatId) ? parsedLeaveChatId : 0L;
var enableAddChatAdminTest = bool.TryParse(Environment.GetEnvironmentVariable("MAX_ENABLE_ADD_CHAT_ADMIN_TEST"), out var parsedAddChatAdminTest) && parsedAddChatAdminTest;
var enableRemoveChatAdminTest = bool.TryParse(Environment.GetEnvironmentVariable("MAX_ENABLE_REMOVE_CHAT_ADMIN_TEST"), out var parsedRemoveChatAdminTest) && parsedRemoveChatAdminTest;
var adminTestUserId = long.TryParse(Environment.GetEnvironmentVariable("MAX_ADMIN_TEST_USER_ID"), out var parsedAdminTestUserId) ? parsedAdminTestUserId : 0L;
var enableDeleteSubscriptionTest = bool.TryParse(Environment.GetEnvironmentVariable("MAX_ENABLE_DELETE_SUBSCRIPTION_TEST"), out var parsedDeleteSubscriptionTest) && parsedDeleteSubscriptionTest;
var deleteSubscriptionUrl = Environment.GetEnvironmentVariable("MAX_DELETE_SUBSCRIPTION_URL");
var enableAnswerCallbackTest = bool.TryParse(Environment.GetEnvironmentVariable("MAX_ENABLE_ANSWER_CALLBACK_TEST"), out var parsedAnswerCallbackTest) && parsedAnswerCallbackTest;
var answerCallbackId = Environment.GetEnvironmentVariable("MAX_ANSWER_CALLBACK_ID");

var services = new ServiceCollection();
services.AddMaxBotClient(C_BOT_API, 30);

await using var temp = TempSession.Create();

try
{
    var serviceProvider = services.BuildServiceProvider();
    var maxApiClient = serviceProvider.GetRequiredService<IMaxBotClient>();

    TempSession.Write("Вызываем GetMeAsync...");
    var me = await maxApiClient.GetMeAsync();
    TempSession.Write($"Успех! Бот: {me.FirstName} (ID: {me.Id}), команды: {string.Join(", ", me.Commands?.Select(c => c.Name) ?? [])}");

    TempSession.Write("Вызываем UpdateBotCommandsAsync...");
    var commandsResponse = await maxApiClient.UpdateBotCommandsAsync(new UpdateBotCommandsRequest
    {
        Commands = new List<BotCommand>
        {
            new() { Name = "start", Description = "Начать работу" },
            new() { Name = "help", Description = "Справка" },
        },
    });
    TempSession.Write($"Команды обновлены: {string.Join(", ", commandsResponse.Commands?.Select(c => c.Name) ?? [])}");

    TempSession.Write("Вызываем SendMessageAsync...");
    var sentMessageResponse = await maxApiClient.SendMessageAsync(new SendMessageRequest()
    {
        ChatId = C_TEST_CHAT_ID,
        Text = "Отправка сообщения",
        Format = MessageFormat.Markdown,
    });

    TempSession.Write("Вызываем SendMessageAsync-Attachment-InlineKeyboardPayload...");
    await maxApiClient.SendMessageAsync(new SendMessageRequest()
    {
        ChatId = C_TEST_CHAT_ID,
        Text = "Отправка сообщения с клавиатурой",
        Format = MessageFormat.Markdown,
        Attachments = new List<Attachment>
        {
            new InlineKeyboardAttachment
            {
                Payload = new InlineKeyboardPayload()
                {
                    Buttons = new List<List<Button>>()
                    {
                        // --- Ряд 1: Две кнопки ---
                        new List<Button>
                        {
                            new LinkButton
                            {
                                Text = "Открыть сайт",
                                Url = "https://saasoft.ru"
                            },
                            new CallbackButton
                            {
                                Text = "Подтвердить",
                                Payload = "confirm_action"
                            }
                        },
                        // --- Ряд 2: Одна большая кнопка ---
                        new List<Button>
                        {
                            new RequestGeoButton
                            {
                                Text = "Отправить геолокацию",
                                Quick = true
                            }
                        },
                        // --- Ряд 3: Одна большая кнопка ---
                        new List<Button>
                        {
                            new MessageButton()
                            {
                                Text = "Отправить текст",
                            }
                        }
                    }
                }
            }
        }
    });

    if (enableAnswerCallbackTest && !string.IsNullOrWhiteSpace(answerCallbackId))
    {
        TempSession.Write("Вызываем AnswerCallbackAsync...");
        var responseAnswerCallback = await maxApiClient.AnswerCallbackAsync(new AnswerCallbackRequest()
        {
            CallbackId = answerCallbackId,
            Notification = "Callback обработан",
            Message = new NewMessageBody
            {
                Text = "Сообщение обновлено после нажатия кнопки",
                Format = MessageFormat.Markdown,
            },
        });
        TempSession.Write($"Callback обработан: {responseAnswerCallback.Success}, сообщение: {responseAnswerCallback.Message}");
    }
    else
    {
        TempSession.Write("AnswerCallbackAsync пропущен: нужен реальный callback_id из события message_callback. Укажите MAX_ANSWER_CALLBACK_ID и включите MAX_ENABLE_ANSWER_CALLBACK_TEST=true.");
    }

    temp.Log("Вызываем GetMessagesAsync...");
    var response = await maxApiClient.GetMessagesAsync(new GetMessagesRequest()
    {
        ChatId = C_TEST_CHAT_ID,
        Count = 50,
    });
    temp.Log($"Получено {response?.Messages?.Length} сообщений:");

    if (response?.Messages is { Length: > 0 } messages)
    {
        foreach (var message in messages)
        {
            temp.Log($"Сообщение {message.Body?.Mid}: {message.Body?.Text}");

            if (message.Body?.Attachments is not { Count: > 0 } attachments)
                continue;

            var index = 0;
            foreach (var attachment in attachments)
            {
                try
                {
                    var result = await maxApiClient.DownloadAttachmentAsync(attachment, new DownloadAttachmentOptions
                    {
                        FilePath = Path.Combine(temp.AttachmentsDirectory, $"{message.Body.Mid}_{index}"),
                        VideoQuality = VideoQuality.Mp4_720,
                    });
                    temp.Log($"  скачано: {Path.GetRelativePath(temp.SessionDirectory, result.SavedFilePath!)}");
                }
                catch (NotSupportedException)
                {
                }

                index++;
            }
        }

        var lastMessageId = messages.LastOrDefault()?.Body?.Mid;
        if (!string.IsNullOrWhiteSpace(lastMessageId))
        {
            temp.Log("Вызываем GetMessageByIdAsync...");
            var responseById = await maxApiClient.GetMessageByIdAsync(lastMessageId);
            temp.Log($"Получено {responseById?.Body?.Text}:");

            if (responseById?.Body?.Attachments is { Count: > 0 } attachmentsById)
            {
                var index = 0;
                foreach (var attachment in attachmentsById)
                {
                    try
                    {
                        var result = await maxApiClient.DownloadAttachmentAsync(attachment, new DownloadAttachmentOptions
                        {
                            VideoQuality = VideoQuality.Mp4_720,
                        });
                        var content = result.Content!;
                        var preview = Encoding.UTF8.GetString(content, 0, Math.Min(100, content.Length));
                        temp.Log($"  получено {content.Length} байт, начало: {preview}");
                    }
                    catch (NotSupportedException)
                    {
                    }

                    index++;
                }
            }

            TempSession.Write("Вызываем EditMessageByIdAsync...");
            var responseEdit = await maxApiClient.EditMessageByIdAsync(lastMessageId, new SendMessageRequest()
            {
                Text = "Изменил ТЕКСТ !!!",
                Format = MessageFormat.Markdown,
            });
            TempSession.Write($"Изменено {responseEdit?.Success}:");
        }

        var firstMessageId = messages.FirstOrDefault()?.Body?.Mid;
        if (!string.IsNullOrWhiteSpace(firstMessageId))
        {
            TempSession.Write("Вызываем DeleteMessageByIdAsync...");
            var responseDelete = await maxApiClient.DeleteMessageByIdAsync(firstMessageId);
            TempSession.Write($"Удалено {responseDelete?.Success}:");
        }

        var largeVideoFilePath = Path.Combine(AppContext.BaseDirectory, "Files", "large_video.mp4");

        TempSession.Write("Вызываем UploadsAsync для large_video.mp4...");
        await using var largeVideoFileContent = File.OpenRead(largeVideoFilePath);
        var largeVideoToken = await maxApiClient.UploadsAsync(new UploadRequest()
        {
            Type = UploadType.Video,
            Content = largeVideoFileContent,
            FileName = Path.GetFileName(largeVideoFilePath),
            ContentType = "video/mp4",
        });
        TempSession.Write($"Получен токен large_video.mp4: {largeVideoToken}");

        TempSession.Write("Отправляем сообщение с large_video.mp4...");
        var largeVideoResponse = await maxApiClient.SendMessageAsync(new SendMessageRequest()
        {
            ChatId = C_TEST_CHAT_ID,
            Text = "Большое видео large_video.mp4",
            Attachments = new List<Attachment>
            {
                new VideoAttachment
                {
                    Payload = new VideoPayload
                    {
                        Token = largeVideoToken,
                    }
                }
            }
        });
        TempSession.Write($"Сообщение с large_video.mp4 отправлено: {largeVideoResponse.Message?.Body?.Mid}");
    }

    temp.Log($"Готово. Логи и вложения: {temp.SessionDirectory}");
    var responseChats = await maxApiClient.GetChatsAsync(new GetChatsRequest()
    {
        Count = 1,
        Marker = null,
    });
    TempSession.Write($"Получено {responseChats?.Chats?.Length} чатов:");

    TempSession.Write("Вызываем GetChatByIdAsync...");
    var responseChat = await maxApiClient.GetChatByIdAsync(C_TEST_CHAT_ID);
    TempSession.Write($"Получен чат: id={responseChat.ChatId}, title={responseChat.Title}, type={responseChat.Type}, status={responseChat.Status}");

    if (!string.IsNullOrWhiteSpace(responseChat.Title))
    {
        TempSession.Write("Вызываем UpdateChatAsync...");
        var responseUpdatedChat = await maxApiClient.UpdateChatAsync(C_TEST_CHAT_ID, new UpdateChatRequest()
        {
            Title = responseChat.Title,
            Notify = false,
        });
        TempSession.Write($"Чат обновлен: id={responseUpdatedChat.ChatId}, title={responseUpdatedChat.Title}, type={responseUpdatedChat.Type}, status={responseUpdatedChat.Status}");
    }
    else
    {
        TempSession.Write("UpdateChatAsync пропущен: у тестового чата нет названия для безопасной проверки без изменения данных.");
    }

    if (enableDeleteChatTest && deleteTestChatId != 0)
    {
        TempSession.Write("Вызываем DeleteChatAsync...");
        var responseDeleteChat = await maxApiClient.DeleteChatAsync(deleteTestChatId);
        TempSession.Write($"Чат удален: {responseDeleteChat.Success}, сообщение: {responseDeleteChat.Message}");
    }
    else
    {
        TempSession.Write("DeleteChatAsync пропущен: метод удаляет групповой чат для всех участников. Укажите отдельный тестовый чат в MAX_DELETE_TEST_CHAT_ID и включите MAX_ENABLE_DELETE_CHAT_TEST=true.");
    }

    TempSession.Write("Вызываем SendChatActionAsync...");
    var responseChatAction = await maxApiClient.SendChatActionAsync(C_TEST_CHAT_ID, new SendChatActionRequest()
    {
        Action = SenderActions.TypingOn,
    });
    TempSession.Write($"Действие бота отправлено: {responseChatAction.Success}, сообщение: {responseChatAction.Message}");

    var sentMessageId = sentMessageResponse.Message?.Body?.Mid;
    if (!string.IsNullOrWhiteSpace(sentMessageId))
    {
        TempSession.Write("Вызываем PinChatMessageAsync...");
        var responsePinMessage = await maxApiClient.PinChatMessageAsync(C_TEST_CHAT_ID, new PinChatMessageRequest()
        {
            MessageId = sentMessageId,
            Notify = false,
        });
        TempSession.Write($"Сообщение закреплено: {responsePinMessage.Success}, сообщение: {responsePinMessage.Message}");
    }
    else
    {
        TempSession.Write("PinChatMessageAsync пропущен: API не вернул ID отправленного сообщения.");
    }

    TempSession.Write("Вызываем GetChatPinnedMessageAsync...");
    var responsePinnedMessage = await maxApiClient.GetChatPinnedMessageAsync(C_TEST_CHAT_ID);
    if (responsePinnedMessage.Message != null)
    {
        TempSession.Write($"Закрепленное сообщение: {responsePinnedMessage.Message.Body?.Mid}, текст: {responsePinnedMessage.Message.Body?.Text}");

        TempSession.Write("Вызываем UnpinChatMessageAsync...");
        var responseUnpinMessage = await maxApiClient.UnpinChatMessageAsync(C_TEST_CHAT_ID);
        TempSession.Write($"Закрепленное сообщение удалено: {responseUnpinMessage.Success}, сообщение: {responseUnpinMessage.Message}");
    }
    else
    {
        TempSession.Write("Закрепленного сообщения нет.");
    }

    TempSession.Write("Вызываем GetChatMembershipAsync...");
    var responseChatMembership = await maxApiClient.GetChatMembershipAsync(C_TEST_CHAT_ID);
    TempSession.Write($"Бот в чате: user_id={responseChatMembership.UserId}, is_bot={responseChatMembership.IsBot}, is_owner={responseChatMembership.IsOwner}, is_admin={responseChatMembership.IsAdmin}, permissions={string.Join(",", responseChatMembership.Permissions ?? new List<string>())}");

    if (enableLeaveChatTest && leaveTestChatId != 0)
    {
        TempSession.Write("Вызываем LeaveChatAsync...");
        var responseLeaveChat = await maxApiClient.LeaveChatAsync(leaveTestChatId);
        TempSession.Write($"Бот удален из чата: {responseLeaveChat.Success}, сообщение: {responseLeaveChat.Message}");
    }
    else
    {
        TempSession.Write("LeaveChatAsync пропущен: метод удаляет текущего бота из группового чата. Укажите отдельный тестовый чат в MAX_LEAVE_TEST_CHAT_ID и включите MAX_ENABLE_LEAVE_CHAT_TEST=true.");
    }

    TempSession.Write("Вызываем GetChatAdminsAsync...");
    var responseChatAdmins = await maxApiClient.GetChatAdminsAsync(C_TEST_CHAT_ID);
    TempSession.Write($"Получено {responseChatAdmins?.Members?.Length} администраторов, marker={responseChatAdmins?.Marker}");

    if (enableAddChatAdminTest && adminTestUserId != 0)
    {
        TempSession.Write("Вызываем AddChatAdminsAsync...");
        var responseAddChatAdmins = await maxApiClient.AddChatAdminsAsync(C_TEST_CHAT_ID, new AddChatAdminsRequest()
        {
            Admins = new List<ChatAdmin>
            {
                new()
                {
                    UserId = adminTestUserId,
                    Permissions = new List<string>
                    {
                        ChatAdminPermission.ReadAllMessages,
                        ChatAdminPermission.PinMessage,
                        ChatAdminPermission.Write,
                    },
                    Alias = "Test admin",
                }
            }
        });
        TempSession.Write($"Администратор назначен: {responseAddChatAdmins.Success}, сообщение: {responseAddChatAdmins.Message}");
    }
    else
    {
        TempSession.Write("AddChatAdminsAsync пропущен: метод меняет права пользователя. Укажите MAX_ADMIN_TEST_USER_ID и включите MAX_ENABLE_ADD_CHAT_ADMIN_TEST=true.");
    }

    if (enableRemoveChatAdminTest && adminTestUserId != 0)
    {
        TempSession.Write("Вызываем RemoveChatAdminAsync...");
        var responseRemoveChatAdmin = await maxApiClient.RemoveChatAdminAsync(C_TEST_CHAT_ID, adminTestUserId);
        TempSession.Write($"Права администратора отменены: {responseRemoveChatAdmin.Success}, сообщение: {responseRemoveChatAdmin.Message}");
    }
    else
    {
        TempSession.Write("RemoveChatAdminAsync пропущен: метод снимает права администратора. Укажите MAX_ADMIN_TEST_USER_ID и включите MAX_ENABLE_REMOVE_CHAT_ADMIN_TEST=true.");
    }

    TempSession.Write("Вызываем GetChatMembersAsync...");
    var responseChatMembers = await maxApiClient.GetChatMembersAsync(new GetChatMembersRequest()
    {
        ChatId = C_TEST_CHAT_ID,
    });
    TempSession.Write($"Получено {responseChatMembers?.Members?.Length} пользователей:");

    TempSession.Write("Вызываем AddChatMemberAsync...");
    var isAdded = await maxApiClient.AddChatMemberAsync(new AddChatMemberRequest()
    {
        ChatId = C_TEST_CHAT_ID,
        UserIds = [C_TEST_USER_ID],
    });

    if (isAdded != null && isAdded.Success)
    {
        TempSession.Write("Пользователь успешно добавлен в чат");
    }

    TempSession.Write("Вызываем DeleteChatMemberAsync...");
    var isDeleted = await maxApiClient.DeleteChatMemberAsync(new DeleteChatMemberRequest()
    {
        ChatId = C_TEST_CHAT_ID,
        UserId = C_TEST_USER_ID,
    });

    if (isDeleted != null && isDeleted.Success)
    {
        TempSession.Write("Пользователь успешно удален из чата");
    }

    TempSession.Write("Проверяем GetMessageByIdAsync для тестового ID...");
    var messageId = response?.Messages?.Last()?.Body?.Mid;
    if (messageId != null)
    {
        var responseMessage = await maxApiClient.GetMessageByIdAsync(messageId);
        TempSession.Write($"Получено сообщение по ID: {responseMessage?.Body?.Text}");
    }

    var filesDirectory = Path.Combine(AppContext.BaseDirectory, "Files");
    var textFilePath = Path.Combine(filesDirectory, "test.txt");
    var videoFilePath = Path.Combine(filesDirectory, "video.mp4");

    TempSession.Write("Вызываем UploadsAsync для test.txt...");
    await using var textFileContent = File.OpenRead(textFilePath);
    var textFileToken = await maxApiClient.UploadsAsync(new UploadRequest()
    {
        Type = UploadType.File,
        Content = textFileContent,
        FileName = Path.GetFileName(textFilePath),
        ContentType = "text/plain",
    });
    TempSession.Write($"Получен токен загруженного файла: {textFileToken}");

    TempSession.Write("Отправляем сообщение с файлом test.txt...");
    await maxApiClient.SendMessageAsync(new SendMessageRequest()
    {
        ChatId = C_TEST_CHAT_ID,
        Text = "Файл test.txt",
        Attachments = new List<Attachment>
        {
            new FileAttachment
            {
                Payload = new FilePayload
                {
                    Token = textFileToken,
                }
            }
        }
    });

    TempSession.Write("Вызываем UploadsAsync для video.mp4...");
    await using var videoFileContent = File.OpenRead(videoFilePath);
    var videoToken = await maxApiClient.UploadsAsync(new UploadRequest()
    {
        Type = UploadType.Video,
        Content = videoFileContent,
        FileName = Path.GetFileName(videoFilePath),
        ContentType = "video/mp4",
    });
    TempSession.Write($"Получен токен загруженного видео: {videoToken}");

    TempSession.Write("Вызываем GetVideoAsync для video.mp4...");
    var videoInfo = await maxApiClient.GetVideoAsync(videoToken);
    TempSession.Write($"Видео: token={videoInfo.Token}, width={videoInfo.Width}, height={videoInfo.Height}, duration={videoInfo.Duration}, url_720={videoInfo.TryGetDownloadUrl(VideoQuality.Mp4_720)}");

    TempSession.Write("Отправляем сообщение с видео video.mp4...");
    await maxApiClient.SendMessageAsync(new SendMessageRequest()
    {
        ChatId = C_TEST_CHAT_ID,
        Text = "Видео video.mp4",
        Attachments = new List<Attachment>
        {
            new VideoAttachment
            {
                Payload = new VideoPayload
                {
                    Token = videoToken,
                }
            }
        }
    });

    TempSession.Write("Вызываем GetUpdatesAsync...");
    var responseUpdates = await maxApiClient.GetUpdatesAsync(new GetUpdatesRequest()
    {
        Timeout = 2,
    });
    TempSession.Write($"Маркер: {responseUpdates?.Marker}, Количество обновлений: {responseUpdates?.Updates.Count}");

    TempSession.Write("Вызываем GetSubscriptionsAsync...");
    var responseSubscriptions = await maxApiClient.GetSubscriptionsAsync();
    TempSession.Write($"Количество Webhook-подписок: {responseSubscriptions.Subscriptions.Count}");

    foreach (var subscription in responseSubscriptions.Subscriptions)
    {
        var updateTypes = subscription.UpdateTypes is { Count: > 0 }
            ? string.Join(", ", subscription.UpdateTypes)
            : "все типы";
        TempSession.Write($"Подписка: url={subscription.Url}, time={subscription.Time}, update_types={updateTypes}");
    }

    if (!string.IsNullOrWhiteSpace(C_TEST_WEBHOOK_URL))
    {
        TempSession.Write("Вызываем SubscribeAsync...");
        var responseSubscribe = await maxApiClient.SubscribeAsync(new SubscriptionRequest()
        {
            Url = C_TEST_WEBHOOK_URL,
            UpdateTypes = new List<string>
            {
                UpdateTypes.MessageCreated,
                UpdateTypes.MessageCallback,
                UpdateTypes.BotStarted,
            },
            Secret = "test-secret_12345",
        });
        TempSession.Write($"Подписка создана: {responseSubscribe.Success}, сообщение: {responseSubscribe.Message}");
    }
    else
    {
        TempSession.Write("SubscribeAsync пропущен: задайте C_TEST_WEBHOOK_URL с публичным HTTPS endpoint.");
    }

    if (enableDeleteSubscriptionTest)
    {
        TempSession.Write("Вызываем UnsubscribeAsync...");
        var responseUnsubscribe = await maxApiClient.UnsubscribeAsync(new DeleteSubscriptionRequest()
        {
            Url = string.IsNullOrWhiteSpace(deleteSubscriptionUrl) ? C_TEST_WEBHOOK_URL : deleteSubscriptionUrl,
        });
        TempSession.Write($"Подписка удалена: {responseUnsubscribe.Success}, сообщение: {responseUnsubscribe.Message}");
    }
    else
    {
        TempSession.Write("UnsubscribeAsync пропущен: метод удаляет Webhook-подписку. Включите MAX_ENABLE_DELETE_SUBSCRIPTION_TEST=true и при необходимости задайте MAX_DELETE_SUBSCRIPTION_URL.");
    }

    // var _ = maxApiClient.PollUpdatesWithCallback(
    //    async (update, client) =>
    //    {
    //        if (update is MessageCreatedUpdate messageCreated)
    //        {
    //            TempSession.Write($"Сообщение: {messageCreated.Message?.Body?.Text}");
    
    //            await client.SendMessageAsync(new SendMessageRequest
    //            {
    //                Text = messageCreated.Message?.Body?.Text,
    //                ChatId = -70581633278133,
    //            });
    //        }
    //    },
    //    limit: 100,
    //    timeout: 90,
    //    types: new List<string> { UpdateTypes.MessageCreated }
    // );
}
catch (Exception ex)
{
    TempSession.Write($"Ошибка: {ex.Message}");
    Environment.Exit(1);
}

sealed class TempSession : IAsyncDisposable
{
    private readonly StreamWriter _logWriter;

    public string SessionDirectory { get; }
    public string AttachmentsDirectory { get; }

    private TempSession(string sessionDirectory)
    {
        Current = this;
        SessionDirectory = sessionDirectory;
        AttachmentsDirectory = Path.Combine(sessionDirectory, "attachments");
        Directory.CreateDirectory(AttachmentsDirectory);

        _logWriter = new StreamWriter(Path.Combine(sessionDirectory, "log.txt"), append: false)
        {
            AutoFlush = true,
        };
    }

    public static TempSession Create()
    {
        var tempDirectory = Path.Combine(GetProjectDirectory(), "Temp");
        var attachmentsDirectory = Path.Combine(tempDirectory, "attachments");

        Directory.CreateDirectory(tempDirectory);
        if (Directory.Exists(attachmentsDirectory))
            Directory.Delete(attachmentsDirectory, recursive: true);
        Directory.CreateDirectory(attachmentsDirectory);

        return new TempSession(tempDirectory);
    }

    internal static TempSession? Current { get; private set; }

    public static void Write(string message)
    {
        if (Current is not null)
            Current.Log(message);
        else
            Console.WriteLine(message);
    }

    private static string GetProjectDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "TestApp.csproj")))
                return directory.FullName;

            directory = directory.Parent;
        }

        return Directory.GetCurrentDirectory();
    }

    public void Log(string message)
    {
        var line = $"[{DateTime.Now:HH:mm:ss}] {message}";
        Console.WriteLine(line);
        _logWriter.WriteLine(line);
    }

    public async ValueTask DisposeAsync()
    {
        await _logWriter.DisposeAsync();
        if (ReferenceEquals(Current, this))
            Current = null;
    }
}
