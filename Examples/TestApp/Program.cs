using MAX.Bot.Extensions;
using MAX.Bot.Interfaces;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Тестирование MaxBotClient...");

Console.WriteLine("\n=== Вариант 1: С использованием DI ===");
var services = new ServiceCollection();
services.AddMaxBotClient("YOUR_TOKEN_BOT", 30);

try
{
    var serviceProvider = services.BuildServiceProvider();
    var diClient = serviceProvider.GetRequiredService<IMaxBotClient>();
    Console.WriteLine("DI клиент создан успешно!");

    Console.WriteLine("Вызываем GetMeAsync...");
    var me = await diClient.GetMeAsync();
    Console.WriteLine($"Успех! Бот: {me.FirstName} (ID: {me.Id})");

    //Console.WriteLine("Вызываем SendMessageAsync...");
    //await diClient.SendMessageAsync(new SendMessageRequest()
    //{
    //    ChatId = -70581633278133,
    //    Text = "Отправка сообщения",
    //    Format = "markdown",
    //});
    //Console.WriteLine("Сообщение спешно отправлено...");

    Console.WriteLine("Вызываем GetMessagesAsync...");
    var response = await diClient.GetMessagesAsync(new GetMessagesRequest()
    {
        ChatId = -70581633278133,
    });

    if (response?.Messages != null && response.Messages.Any())
    {
        Console.WriteLine($"Получено {response.Messages.Length} сообщений:");

        foreach (var message in response.Messages)
        {
            var text = message?.Body?.Text ?? "[нет текста или сообщение другого типа]";
            Console.WriteLine($"Текст: {text}");
        }
    }
    else
    {
        Console.WriteLine("Сообщения не получены. Ответ null или массив пуст.");

        if (response != null)
        {
            Console.WriteLine($"Response объект получен, но Messages = {response.Messages?.Length ?? 0}");
        }
    }

    var responseChats = await diClient.GetChatsAsync(new GetChatsRequest()
    {
        Count = 1,
        Marker = null,
    });

    if (responseChats?.Chats != null && responseChats.Chats.Any())
    {
        Console.WriteLine($"Получено {responseChats?.Chats.Length} чатов:");

        foreach (var chat in responseChats?.Chats)
        {
            var chatName = chat.ChatId.ToString();
            Console.WriteLine($"ChatID: {chatName}");
        }
    }

    var responseChatMembers = await diClient.GetChatMembersAsync(new GetChatMembersRequest()
    {
        ChatId = -70581633278133,
    });

    if (responseChatMembers?.Members != null && responseChatMembers.Members.Any())
    {
        Console.WriteLine($"Получено {responseChatMembers?.Members.Length} пользователей:");
        foreach (var member in responseChatMembers?.Members)
        {
            Console.WriteLine($"Пользователь: {member.FirstName}");
        }
    }

    var isAdded = await diClient.AddChatMemberAsync(new AddChatMemberRequest()
    {
        ChatId = -70581633278133,
        UserIds = [168973682],
    });

    if (isAdded != null && isAdded.Success)
    {
        Console.WriteLine("Пользователь успешно добавлен в чат");
    }
    else
    {
        Console.WriteLine($"Неудачное удаление: {isAdded?.Message}");
    }

    var isDeleted = await diClient.DeleteChatMemberAsync(new DeleteChatMemberRequest()
    {
        ChatId = -70581633278133,
        UserId = 168973682,
    });

    if (isDeleted != null && isDeleted.Success)
    {
        Console.WriteLine("Пользователь успешно удален из чата");
    }
    else
    {
        Console.WriteLine($"Неудачное удаление: {isDeleted?.Message}");
    }

    //Console.WriteLine("Вызываем GetMessageByIdAsync...");
    //var responseMessage = await diClient.GetMessageByIdAsync("1234");
    //Console.WriteLine($"Текст: {responseMessage?.Body?.Text}");

    var _ = diClient.PollUpdatesWithCallback(
        async (update, client) =>
        {
            Console.WriteLine($"Сообщение: {update?.Message?.Body?.Text}");

            if (update?.UpdateType == UpdateTypes.MessageCreated)
            {
                await client.SendMessageAsync(new SendMessageRequest
                {
                    Text = update.Message?.Body?.Text,
                    ChatId = -70581633278133,
                });
            }
        },
        limit: 100,
        timeout: 90,
        types: new List<string> { UpdateTypes.MessageCreated }
    );
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка: {ex.Message}");
}

Console.WriteLine("\nНажмите любую клавишу для выхода...");
Console.ReadKey();