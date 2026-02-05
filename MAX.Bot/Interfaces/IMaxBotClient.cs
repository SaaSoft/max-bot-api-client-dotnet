using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace MAX.Bot.Interfaces;

/// <summary>
/// Интерфейс клиента для работы с API MAX Bot
/// </summary>
public interface IMaxBotClient
{
    /// <summary>
    /// Получить информацию о текущем боте
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Информация о боте</returns>
    Task<User> GetMeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправить сообщение
    /// </summary>
    /// <param name="request">Запрос на отправку сообщения</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Ответ с отправленным сообщением</returns>
    Task<SendMessageResponse> SendMessageAsync(SendMessageRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить обновления (события)
    /// </summary>
    /// <param name="request">Запрос на получение обновлений</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Ответ с обновлениями</returns>
    Task<GetUpdatesResponse> GetUpdatesAsync(GetUpdatesRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить сообщения из чата
    /// </summary>
    /// <param name="request">Запрос на получение сообщений</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Ответ с сообщениями</returns>
    Task<GetMessagesResponse> GetMessagesAsync(GetMessagesRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить сообщение по идентификатору
    /// </summary>
    /// <param name="messageId">Идентификатор сообщения</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Сообщение</returns>
    Task<Message> GetMessageByIdAsync(string messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить список чатов
    /// </summary>
    /// <param name="request">Запрос на получение чатов</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Ответ со списком чатов</returns>
    Task<GetChatsResponse> GetChatsAsync(GetChatsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить участников чата
    /// </summary>
    /// <param name="request">Запрос на получение участников чата</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Ответ со списком участников</returns>
    Task<GetChatMembersResponse> GetChatMembersAsync(GetChatMembersRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить участника из чата
    /// </summary>
    /// <param name="request">Запрос на удаление участника</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Базовый ответ операции</returns>
    Task<BaseResponse> DeleteChatMemberAsync(DeleteChatMemberRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить участника в чат
    /// </summary>
    /// <param name="request">Запрос на добавление участника</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Базовый ответ операции</returns>
    Task<BaseResponse> AddChatMemberAsync(AddChatMemberRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Долгосрочный опрос обновлений с обработкой через callback-функцию
    /// </summary>
    /// <param name="callback">Функция обратного вызова для обработки каждого обновления</param>
    /// <param name="limit">Максимальное количество обновлений за один запрос</param>
    /// <param name="timeout">Таймаут ожидания обновлений в секундах</param>
    /// <param name="marker">Маркер для получения обновлений после определенной точки</param>
    /// <param name="types">Типы обновлений для фильтрации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Асинхронная задача</returns>
    Task PollUpdatesWithCallback(
        Func<Update, IMaxBotClient, Task> callback,
        int? limit = 100,
        int? timeout = 30,
        long? marker = null,
        List<string>? types = null,
        CancellationToken cancellationToken = default);
}