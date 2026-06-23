using MAX.Bot.Exceptions;
using MAX.Bot.Extensions;
using MAX.Bot.Interfaces;
using MAX.Bot.Interfaces.Models;
using MAX.Bot.Interfaces.Models.Request;
using MAX.Bot.Interfaces.Models.Request.Message;
using MAX.Bot.Interfaces.Models.Request.Message.Attachment;
using MAX.Bot.Interfaces.Models.Request.Message.Attachment.Payloads;
using MAX.Bot.Interfaces.Models.Response;
using Microsoft.Extensions.DependencyInjection;

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

try
{
    var serviceProvider = services.BuildServiceProvider();
    var maxApiClient = serviceProvider.GetRequiredService<IMaxBotClient>();

    Console.WriteLine("Вызываем GetMeAsync...");
    var me = await maxApiClient.GetMeAsync();
    Console.WriteLine($"Успех! Бот: {me.FirstName} (ID: {me.Id})");
    
    Console.WriteLine("Вызываем SendMessageAsync...");
    var sentMessageResponse = await maxApiClient.SendMessageAsync(new SendMessageRequest()
    {
        ChatId = C_TEST_CHAT_ID,
        Text = "Отправка сообщения",
        Format = MessageFormat.Markdown,
    });

    Console.WriteLine("Вызываем SendMessageAsync-Attachment-InlineKeyboardPayload...");
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
        Console.WriteLine("Вызываем AnswerCallbackAsync...");
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
        Console.WriteLine($"Callback обработан: {responseAnswerCallback.Success}, сообщение: {responseAnswerCallback.Message}");
    }
    else
    {
        Console.WriteLine("AnswerCallbackAsync пропущен: нужен реальный callback_id из события message_callback. Укажите MAX_ANSWER_CALLBACK_ID и включите MAX_ENABLE_ANSWER_CALLBACK_TEST=true.");
    }

    Console.WriteLine("Вызываем GetMessagesAsync...");
    var response = await maxApiClient.GetMessagesAsync(new GetMessagesRequest()
    {
        ChatId = C_TEST_CHAT_ID,
        Count = 50,
    });
    Console.WriteLine($"Получено {response?.Messages?.Length} сообщений:");
    
    if (response?.Messages is { Length: > 0 } messages)
    {
        var lastMessageId = messages.LastOrDefault()?.Body?.Mid;
        if (!string.IsNullOrWhiteSpace(lastMessageId))
        {
            Console.WriteLine("Вызываем GetMessageByIdAsync...");
            var responseById = await maxApiClient.GetMessageByIdAsync(lastMessageId);
            Console.WriteLine($"Получено {responseById?.Body?.Text}:");

            Console.WriteLine("Вызываем EditMessageByIdAsync...");
            var responseEdit = await maxApiClient.EditMessageByIdAsync(lastMessageId, new SendMessageRequest()
            {
                Text = "Изменил ТЕКСТ !!!",
                Format = MessageFormat.Markdown,
            });
            Console.WriteLine($"Изменено {responseEdit?.Success}:");
        }

        var firstMessageId = messages.FirstOrDefault()?.Body?.Mid;
        if (!string.IsNullOrWhiteSpace(firstMessageId))
        {
            Console.WriteLine("Вызываем DeleteMessageByIdAsync...");
            var responseDelete = await maxApiClient.DeleteMessageByIdAsync(firstMessageId);
            Console.WriteLine($"Удалено {responseDelete?.Success}:");
        }
    }

    Console.WriteLine("Вызываем GetChatsAsync...");
    var responseChats = await maxApiClient.GetChatsAsync(new GetChatsRequest()
    {
        Count = 1,
        Marker = null,
    });
    Console.WriteLine($"Получено {responseChats?.Chats?.Length} чатов:");

    Console.WriteLine("Вызываем GetChatByIdAsync...");
    var responseChat = await maxApiClient.GetChatByIdAsync(C_TEST_CHAT_ID);
    Console.WriteLine($"Получен чат: id={responseChat.ChatId}, title={responseChat.Title}, type={responseChat.Type}, status={responseChat.Status}");

    if (!string.IsNullOrWhiteSpace(responseChat.Title))
    {
        Console.WriteLine("Вызываем UpdateChatAsync...");
        var responseUpdatedChat = await maxApiClient.UpdateChatAsync(C_TEST_CHAT_ID, new UpdateChatRequest()
        {
            Title = responseChat.Title,
            Notify = false,
        });
        Console.WriteLine($"Чат обновлен: id={responseUpdatedChat.ChatId}, title={responseUpdatedChat.Title}, type={responseUpdatedChat.Type}, status={responseUpdatedChat.Status}");
    }
    else
    {
        Console.WriteLine("UpdateChatAsync пропущен: у тестового чата нет названия для безопасной проверки без изменения данных.");
    }

    if (enableDeleteChatTest && deleteTestChatId != 0)
    {
        Console.WriteLine("Вызываем DeleteChatAsync...");
        var responseDeleteChat = await maxApiClient.DeleteChatAsync(deleteTestChatId);
        Console.WriteLine($"Чат удален: {responseDeleteChat.Success}, сообщение: {responseDeleteChat.Message}");
    }
    else
    {
        Console.WriteLine("DeleteChatAsync пропущен: метод удаляет групповой чат для всех участников. Укажите отдельный тестовый чат в MAX_DELETE_TEST_CHAT_ID и включите MAX_ENABLE_DELETE_CHAT_TEST=true.");
    }

    Console.WriteLine("Вызываем SendChatActionAsync...");
    var responseChatAction = await maxApiClient.SendChatActionAsync(C_TEST_CHAT_ID, new SendChatActionRequest()
    {
        Action = SenderActions.TypingOn,
    });
    Console.WriteLine($"Действие бота отправлено: {responseChatAction.Success}, сообщение: {responseChatAction.Message}");

    var sentMessageId = sentMessageResponse.Message?.Body?.Mid;
    if (!string.IsNullOrWhiteSpace(sentMessageId))
    {
        Console.WriteLine("Вызываем PinChatMessageAsync...");
        var responsePinMessage = await maxApiClient.PinChatMessageAsync(C_TEST_CHAT_ID, new PinChatMessageRequest()
        {
            MessageId = sentMessageId,
            Notify = false,
        });
        Console.WriteLine($"Сообщение закреплено: {responsePinMessage.Success}, сообщение: {responsePinMessage.Message}");
    }
    else
    {
        Console.WriteLine("PinChatMessageAsync пропущен: API не вернул ID отправленного сообщения.");
    }

    Console.WriteLine("Вызываем GetChatPinnedMessageAsync...");
    var responsePinnedMessage = await maxApiClient.GetChatPinnedMessageAsync(C_TEST_CHAT_ID);
    if (responsePinnedMessage.Message != null)
    {
        Console.WriteLine($"Закрепленное сообщение: {responsePinnedMessage.Message.Body?.Mid}, текст: {responsePinnedMessage.Message.Body?.Text}");

        Console.WriteLine("Вызываем UnpinChatMessageAsync...");
        var responseUnpinMessage = await maxApiClient.UnpinChatMessageAsync(C_TEST_CHAT_ID);
        Console.WriteLine($"Закрепленное сообщение удалено: {responseUnpinMessage.Success}, сообщение: {responseUnpinMessage.Message}");
    }
    else
    {
        Console.WriteLine("Закрепленного сообщения нет.");
    }
    
    Console.WriteLine("Вызываем GetChatMembershipAsync...");
    var responseChatMembership = await maxApiClient.GetChatMembershipAsync(C_TEST_CHAT_ID);
    Console.WriteLine($"Бот в чате: user_id={responseChatMembership.UserId}, is_bot={responseChatMembership.IsBot}, is_owner={responseChatMembership.IsOwner}, is_admin={responseChatMembership.IsAdmin}, permissions={string.Join(",", responseChatMembership.Permissions ?? new List<string>())}");

    if (enableLeaveChatTest && leaveTestChatId != 0)
    {
        Console.WriteLine("Вызываем LeaveChatAsync...");
        var responseLeaveChat = await maxApiClient.LeaveChatAsync(leaveTestChatId);
        Console.WriteLine($"Бот удален из чата: {responseLeaveChat.Success}, сообщение: {responseLeaveChat.Message}");
    }
    else
    {
        Console.WriteLine("LeaveChatAsync пропущен: метод удаляет текущего бота из группового чата. Укажите отдельный тестовый чат в MAX_LEAVE_TEST_CHAT_ID и включите MAX_ENABLE_LEAVE_CHAT_TEST=true.");
    }

    Console.WriteLine("Вызываем GetChatAdminsAsync...");
    var responseChatAdmins = await maxApiClient.GetChatAdminsAsync(C_TEST_CHAT_ID);
    Console.WriteLine($"Получено {responseChatAdmins?.Members?.Length} администраторов, marker={responseChatAdmins?.Marker}");

    if (enableAddChatAdminTest && adminTestUserId != 0)
    {
        Console.WriteLine("Вызываем AddChatAdminsAsync...");
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
        Console.WriteLine($"Администратор назначен: {responseAddChatAdmins.Success}, сообщение: {responseAddChatAdmins.Message}");
    }
    else
    {
        Console.WriteLine("AddChatAdminsAsync пропущен: метод меняет права пользователя. Укажите MAX_ADMIN_TEST_USER_ID и включите MAX_ENABLE_ADD_CHAT_ADMIN_TEST=true.");
    }

    if (enableRemoveChatAdminTest && adminTestUserId != 0)
    {
        Console.WriteLine("Вызываем RemoveChatAdminAsync...");
        var responseRemoveChatAdmin = await maxApiClient.RemoveChatAdminAsync(C_TEST_CHAT_ID, adminTestUserId);
        Console.WriteLine($"Права администратора отменены: {responseRemoveChatAdmin.Success}, сообщение: {responseRemoveChatAdmin.Message}");
    }
    else
    {
        Console.WriteLine("RemoveChatAdminAsync пропущен: метод снимает права администратора. Укажите MAX_ADMIN_TEST_USER_ID и включите MAX_ENABLE_REMOVE_CHAT_ADMIN_TEST=true.");
    }

    Console.WriteLine("Вызываем GetChatMembersAsync...");
    var responseChatMembers = await maxApiClient.GetChatMembersAsync(new GetChatMembersRequest()
    {
        ChatId = C_TEST_CHAT_ID,
    });
    Console.WriteLine($"Получено {responseChatMembers?.Members?.Length} пользователей:");

    Console.WriteLine("Вызываем AddChatMemberAsync...");
    var isAdded = await maxApiClient.AddChatMemberAsync(new AddChatMemberRequest()
    {
        ChatId = C_TEST_CHAT_ID,
        UserIds = [C_TEST_USER_ID],
    });

    if (isAdded != null && isAdded.Success)
    {
        Console.WriteLine("Пользователь успешно добавлен в чат");
    }

    Console.WriteLine("Вызываем DeleteChatMemberAsync...");
    var isDeleted = await maxApiClient.DeleteChatMemberAsync(new DeleteChatMemberRequest()
    {
        ChatId = C_TEST_CHAT_ID,
        UserId = C_TEST_USER_ID,
    });

    if (isDeleted != null && isDeleted.Success)
    {
        Console.WriteLine("Пользователь успешно удален из чата");
    }

    Console.WriteLine("Проверяем GetMessageByIdAsync для тестового ID...");
    var messageId = response?.Messages?.Last()?.Body?.Mid;
    if (messageId != null)
    {
        var responseMessage = await maxApiClient.GetMessageByIdAsync(messageId);
        Console.WriteLine($"Получено сообщение по ID: {responseMessage?.Body?.Text}");
    }

    var filesDirectory = Path.Combine(AppContext.BaseDirectory, "Files");
    var textFilePath = Path.Combine(filesDirectory, "test.txt");
    var videoFilePath = Path.Combine(filesDirectory, "video.mp4");

    Console.WriteLine("Вызываем UploadsAsync для test.txt...");
    await using var textFileContent = File.OpenRead(textFilePath);
    var textFileToken = await maxApiClient.UploadsAsync(new UploadRequest()
    {
        Type = UploadType.File,
        Content = textFileContent,
        FileName = Path.GetFileName(textFilePath),
        ContentType = "text/plain",
    });
    Console.WriteLine($"Получен токен загруженного файла: {textFileToken}");

    Console.WriteLine("Отправляем сообщение с файлом test.txt...");
    await SendMessageWithAttachmentRetry(maxApiClient, new SendMessageRequest()
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

    Console.WriteLine("Вызываем GetMessagesAsync...");
    response = await maxApiClient.GetMessagesAsync(new GetMessagesRequest()
    {
        ChatId = C_TEST_CHAT_ID,
        Count = 1,
    });
    Console.WriteLine($"Получено {response?.Messages?.Length} сообщений:");

    if (response?.Messages is { Length: > 0 } m)
    {
        var mid = m.FirstOrDefault()?.Body?.Mid;
        if (!string.IsNullOrWhiteSpace(mid))
        {
            Console.WriteLine("Вызываем GetMessageByIdAsync...");
            var responseById = await maxApiClient.GetMessageByIdAsync(mid);
            Console.WriteLine($"Получено {responseById?.Body?.Text}:");

            if (responseById?.Body?.Attachments is { Length: > 0 } attachments)
            {
                if (attachments[0] is FileAttachment fileAttachment)
                {
                    Console.WriteLine($"Вложение файла: {fileAttachment.Payload.Token}");
                }
                else
                {
                    Console.WriteLine("Что-то пошло не так. Вложение файла не найдено.");
                }
            }
        }
    }

    Console.WriteLine("Вызываем UploadsAsync для video.mp4...");
    await using var videoFileContent = File.OpenRead(videoFilePath);
    var videoToken = await maxApiClient.UploadsAsync(new UploadRequest()
    {
        Type = UploadType.Video,
        Content = videoFileContent,
        FileName = Path.GetFileName(videoFilePath),
        ContentType = "video/mp4",
    });
    Console.WriteLine($"Получен токен загруженного видео: {videoToken}");

    Console.WriteLine("Вызываем GetVideoAsync для video.mp4...");
    var videoInfo = await GetVideoInfoWithRetry(maxApiClient, videoToken);
    Console.WriteLine($"Видео: token={videoInfo.Token}, width={videoInfo.Width}, height={videoInfo.Height}, duration={videoInfo.Duration}");

    Console.WriteLine("Отправляем сообщение с видео video.mp4...");
    await SendMessageWithAttachmentRetry(maxApiClient, new SendMessageRequest()
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

    Console.WriteLine("Вызываем GetUpdatesAsync...");
    var responseUpdates = await maxApiClient.GetUpdatesAsync(new GetUpdatesRequest()
    {
        Timeout = 2,
    });
    Console.WriteLine($"Маркер: {responseUpdates?.Marker}, Количество обновлений: {responseUpdates?.Updates.Count}");

    Console.WriteLine("Вызываем GetSubscriptionsAsync...");
    var responseSubscriptions = await maxApiClient.GetSubscriptionsAsync();
    Console.WriteLine($"Количество Webhook-подписок: {responseSubscriptions.Subscriptions.Count}");

    foreach (var subscription in responseSubscriptions.Subscriptions)
    {
        var updateTypes = subscription.UpdateTypes is { Count: > 0 }
            ? string.Join(", ", subscription.UpdateTypes)
            : "все типы";
        Console.WriteLine($"Подписка: url={subscription.Url}, time={subscription.Time}, update_types={updateTypes}");
    }

    if (!string.IsNullOrWhiteSpace(C_TEST_WEBHOOK_URL))
    {
        Console.WriteLine("Вызываем SubscribeAsync...");
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
        Console.WriteLine($"Подписка создана: {responseSubscribe.Success}, сообщение: {responseSubscribe.Message}");
    }
    else
    {
        Console.WriteLine("SubscribeAsync пропущен: задайте C_TEST_WEBHOOK_URL с публичным HTTPS endpoint.");
    }

    if (enableDeleteSubscriptionTest)
    {
        Console.WriteLine("Вызываем UnsubscribeAsync...");
        var responseUnsubscribe = await maxApiClient.UnsubscribeAsync(new DeleteSubscriptionRequest()
        {
            Url = string.IsNullOrWhiteSpace(deleteSubscriptionUrl) ? C_TEST_WEBHOOK_URL : deleteSubscriptionUrl,
        });
        Console.WriteLine($"Подписка удалена: {responseUnsubscribe.Success}, сообщение: {responseUnsubscribe.Message}");
    }
    else
    {
        Console.WriteLine("UnsubscribeAsync пропущен: метод удаляет Webhook-подписку. Включите MAX_ENABLE_DELETE_SUBSCRIPTION_TEST=true и при необходимости задайте MAX_DELETE_SUBSCRIPTION_URL.");
    }

    //var _ = maxApiClient.PollUpdatesWithCallback(
    //    async (update, client) =>
    //    {
    //        if (update is MessageCreatedUpdate messageCreated)
    //        {
    //            Console.WriteLine($"Сообщение: {messageCreated.Message?.Body?.Text}");
    //
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
    //);
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка: {ex.Message}");
    Environment.Exit(1);
}

static async Task SendMessageWithAttachmentRetry(IMaxBotClient maxApiClient, SendMessageRequest request)
{
    var retryDelays = new[]
    {
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
    };

    for (var attempt = 0; ; attempt++)
    {
        try
        {
            await maxApiClient.SendMessageAsync(request);
            return;
        }
        catch (MaxBotClientException ex) when (IsAttachmentNotReady(ex) && attempt < retryDelays.Length)
        {
            var delay = retryDelays[attempt];
            Console.WriteLine($"Вложение еще обрабатывается, повтор через {delay.TotalSeconds:0} сек...");
            await Task.Delay(delay);
        }
    }
}

static bool IsAttachmentNotReady(MaxBotClientException ex)
{
    return ex.Message.Contains("attachment.not.ready", StringComparison.OrdinalIgnoreCase) ||
           ex.Message.Contains("not.processed", StringComparison.OrdinalIgnoreCase);
}

static async Task<VideoInfoResponse> GetVideoInfoWithRetry(IMaxBotClient maxApiClient, string videoToken)
{
    var retryDelays = new[]
    {
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
    };

    for (var attempt = 0; ; attempt++)
    {
        try
        {
            return await maxApiClient.GetVideoAsync(videoToken);
        }
        catch (MaxBotClientException ex) when (IsAttachmentNotReady(ex) && attempt < retryDelays.Length)
        {
            var delay = retryDelays[attempt];
            Console.WriteLine($"Видео еще обрабатывается, повтор через {delay.TotalSeconds:0} сек...");
            await Task.Delay(delay);
        }
    }
}
