using MAX.Bot.Extensions;
using MAX.Bot.Interfaces;
//using MAX.Bot.Interfaces.Models;
using MAX.Bot.Interfaces.Models.Request;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddMaxBotClient("YOUR_BOT_TOKEN", 30);

try
{
    var serviceProvider = services.BuildServiceProvider();
    var maxApiClient = serviceProvider.GetRequiredService<IMaxBotClient>();

    Console.WriteLine("Вызываем GetMeAsync...");
    var me = await maxApiClient.GetMeAsync();
    Console.WriteLine($"Успех! Бот: {me.FirstName} (ID: {me.Id})");

    Console.WriteLine("Вызываем SendMessageAsync...");
    await maxApiClient.SendMessageAsync(new SendMessageRequest()
    {
        ChatId = -70581633278133,
        Text = "Отправка сообщения",
        Format = MessageFormat.Markdown,
    });

    Console.WriteLine("Вызываем GetMessagesAsync...");
    var response = await maxApiClient.GetMessagesAsync(new GetMessagesRequest()
    {
        ChatId = -70581633278133,
    });
    Console.WriteLine($"Получено {response?.Messages?.Length} сообщений:");

    Console.WriteLine("Вызываем GetChatsAsync...");
    var responseChats = await maxApiClient.GetChatsAsync(new GetChatsRequest()
    {
        Count = 1,
        Marker = null,
    });
    Console.WriteLine($"Получено {responseChats?.Chats?.Length} чатов:");

    Console.WriteLine("Вызываем GetChatMembersAsync...");
    var responseChatMembers = await maxApiClient.GetChatMembersAsync(new GetChatMembersRequest()
    {
        ChatId = -70581633278133,
    });
    Console.WriteLine($"Получено {responseChatMembers?.Members?.Length} пользователей:");

    Console.WriteLine("Вызываем AddChatMemberAsync...");
    var isAdded = await maxApiClient.AddChatMemberAsync(new AddChatMemberRequest()
    {
        ChatId = -70581633278133,
        UserIds = [168973682],
    });

    if (isAdded != null && isAdded.Success)
    {
        Console.WriteLine("Пользователь успешно добавлен в чат");
    }

    Console.WriteLine("Вызываем DeleteChatMemberAsync...");
    var isDeleted = await maxApiClient.DeleteChatMemberAsync(new DeleteChatMemberRequest()
    {
        ChatId = -70581633278133,
        UserId = 168973682,
    });

    if (isDeleted != null && isDeleted.Success)
    {
        Console.WriteLine("Пользователь успешно удален из чата");
    }

    Console.WriteLine("Вызываем GetMessageByIdAsync...");
    var responseMessage = await maxApiClient.GetMessageByIdAsync("mid.ffffbfce6ed21f4b019c2841060d67ac");
    Console.WriteLine($"Получено сообщение по ID: {responseMessage?.Body?.Text}");

    Console.WriteLine("Вызываем GetUpdatesAsync...");
    var responseUpdates = await maxApiClient.GetUpdatesAsync(new GetUpdatesRequest()
    {
        Timeout = 2,
    });
    Console.WriteLine($"Маркер: {responseUpdates?.Marker}, Количество обновлений: {responseUpdates?.Updates.Count}");


    //var _ = maxApiClient.PollUpdatesWithCallback(
    //    async (update, client) =>
    //    {
    //        Console.WriteLine($"Сообщение: {update?.Message?.Body?.Text}");

    //        if (update?.UpdateType == UpdateTypes.MessageCreated)
    //        {
    //            await client.SendMessageAsync(new SendMessageRequest
    //            {
    //                Text = update.Message?.Body?.Text,
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
