using MAX.Bot.Interfaces.Models;
using MAX.Bot.Interfaces.Models.Attachment;
using MAX.Bot.Interfaces.Models.Request;
using MAX.Bot.Interfaces.Models.Request.Message;
using MAX.Bot.Interfaces.Models.Request.Message.Attachment;
using MAX.Bot.Interfaces.Models.Response;

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
    /// Добавить, изменить или удалить команды бота. Чтобы удалить все команды, передайте пустой список.
    /// </summary>
    /// <param name="request">Запрос на редактирование команд</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Актуальный список команд бота</returns>
    Task<UpdateBotCommandsResponse> UpdateBotCommandsAsync(
        UpdateBotCommandsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправить сообщение. При отправке с upload-вложениями автоматически повторяет запрос, если API возвращает ошибку «вложение ещё обрабатывается».
    /// </summary>
    /// <param name="request">Запрос на отправку сообщения</param>
    /// <param name="retryOptions">Параметры повторных попыток. Если не указаны, используются настройки клиента.</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Ответ с отправленным сообщением</returns>
    Task<SendMessageResponse> SendMessageAsync(
        SendMessageRequest request,
        AttachmentRetryOptions? retryOptions = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ответить на callback после нажатия пользователем кнопки
    /// </summary>
    /// <param name="request">Запрос на ответ callback</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Базовый ответ операции</returns>
    Task<BaseResponse> AnswerCallbackAsync(AnswerCallbackRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить обновления (события)
    /// </summary>
    /// <param name="request">Запрос на получение обновлений</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Ответ с обновлениями</returns>
    Task<GetUpdatesResponse> GetUpdatesAsync(GetUpdatesRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все подписки через Webhook
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Ответ со списком текущих подписок</returns>
    Task<GetSubscriptionsResponse> GetSubscriptionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Подписаться на обновления о новых событиях через Webhook
    /// </summary>
    /// <param name="request">Запрос на настройку подписки Webhook</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Базовый ответ операции</returns>
    Task<BaseResponse> SubscribeAsync(SubscriptionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отписаться от обновлений о новых событиях через Webhook
    /// </summary>
    /// <param name="request">Запрос на удаление подписки Webhook</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Базовый ответ операции</returns>
    Task<BaseResponse> UnsubscribeAsync(DeleteSubscriptionRequest request, CancellationToken cancellationToken = default);

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
    /// Получить информацию о видео по токену вложения. Автоматически повторяет запрос, если видео ещё обрабатывается.
    /// </summary>
    /// <param name="videoToken">Токен видео-вложения</param>
    /// <param name="retryOptions">Параметры повторных попыток. Если не указаны, используются настройки клиента.</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Информация о видео</returns>
    Task<VideoInfoResponse> GetVideoAsync(
        string videoToken,
        AttachmentRetryOptions? retryOptions = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает URL для скачивания вложения или null, если оно ещё не готово.
    /// </summary>
    /// <param name="attachment">Вложение из тела сообщения</param>
    /// <param name="videoQuality">Качество видео. Используется только для <see cref="VideoAttachment"/>.</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>URL для скачивания или null, если вложение ещё обрабатывается</returns>
    Task<string?> TryGetAttachmentDownloadUrlAsync(
        Attachment attachment,
        VideoQuality videoQuality = VideoQuality.Mp4_480,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Скачать вложение: получить содержимое в память или сохранить на диск.
    /// </summary>
    /// <param name="attachment">Вложение из тела сообщения</param>
    /// <param name="options">Параметры скачивания и повторных попыток</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с содержимым файла или путём к сохранённому файлу</returns>
    Task<AttachmentDownloadResult> DownloadAttachmentAsync(
        Attachment attachment,
        DownloadAttachmentOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Отредактировать(изменить) сообщение по идентификатору. При редактировании с upload-вложениями автоматически повторяет запрос, если API возвращает ошибку «вложение ещё обрабатывается».
    /// </summary>
    /// <param name="messageId">Идентификатор редактируемого сообщения</param>
    /// <param name="messageRequest">Новое сообщение</param>
    /// <param name="retryOptions">Параметры повторных попыток. Если не указаны, используются настройки клиента.</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Базовый ответ операции</returns>
    Task<BaseResponse> EditMessageByIdAsync(
        string messageId,
        SendMessageRequest messageRequest,
        AttachmentRetryOptions? retryOptions = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить сообщение по идентификатору
    /// </summary>
    /// <param name="messageId">Идентификатор сообщения</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Базовый ответ операции</returns>
    Task<BaseResponse> DeleteMessageByIdAsync(string messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить список чатов
    /// </summary>
    /// <param name="request">Запрос на получение чатов</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Ответ со списком чатов</returns>
    Task<GetChatsResponse> GetChatsAsync(GetChatsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить информацию о групповом чате по идентификатору
    /// </summary>
    /// <param name="chatId">Идентификатор запрашиваемого чата</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Информация о чате</returns>
    Task<Chat> GetChatByIdAsync(long chatId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Изменить информацию о групповом чате
    /// </summary>
    /// <param name="chatId">Идентификатор изменяемого чата</param>
    /// <param name="request">Запрос на изменение информации о чате</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Информация о чате после изменения</returns>
    Task<Chat> UpdateChatAsync(long chatId, UpdateChatRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить групповой чат для всех участников
    /// </summary>
    /// <param name="chatId">Идентификатор удаляемого чата</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Базовый ответ операции</returns>
    Task<BaseResponse> DeleteChatAsync(long chatId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправить действие бота в групповой чат
    /// </summary>
    /// <param name="chatId">Идентификатор чата</param>
    /// <param name="request">Запрос на отправку действия бота</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Базовый ответ операции</returns>
    Task<BaseResponse> SendChatActionAsync(long chatId, SendChatActionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить закрепленное сообщение в групповом чате
    /// </summary>
    /// <param name="chatId">Идентификатор чата</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Ответ с закрепленным сообщением</returns>
    Task<GetChatPinnedMessageResponse> GetChatPinnedMessageAsync(long chatId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Закрепить сообщение в групповом чате
    /// </summary>
    /// <param name="chatId">Идентификатор чата</param>
    /// <param name="request">Запрос на закрепление сообщения</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Базовый ответ операции</returns>
    Task<BaseResponse> PinChatMessageAsync(long chatId, PinChatMessageRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить закрепленное сообщение в групповом чате
    /// </summary>
    /// <param name="chatId">Идентификатор чата</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Базовый ответ операции</returns>
    Task<BaseResponse> UnpinChatMessageAsync(long chatId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить информацию о членстве текущего бота в групповом чате
    /// </summary>
    /// <param name="chatId">Идентификатор чата</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Информация о членстве текущего бота в чате</returns>
    Task<ChatMember> GetChatMembershipAsync(long chatId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить текущего бота из группового чата
    /// </summary>
    /// <param name="chatId">Идентификатор чата</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Базовый ответ операции</returns>
    Task<BaseResponse> LeaveChatAsync(long chatId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить список администраторов группового чата
    /// </summary>
    /// <param name="chatId">Идентификатор чата</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Ответ со списком администраторов чата</returns>
    Task<GetChatMembersResponse> GetChatAdminsAsync(long chatId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Назначить администраторов группового чата
    /// </summary>
    /// <param name="chatId">Идентификатор чата</param>
    /// <param name="request">Запрос на назначение администраторов</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Базовый ответ операции</returns>
    Task<BaseResponse> AddChatAdminsAsync(long chatId, AddChatAdminsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отменить права администратора в групповом чате
    /// </summary>
    /// <param name="chatId">Идентификатор чата</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Базовый ответ операции</returns>
    Task<BaseResponse> RemoveChatAdminAsync(long chatId, long userId, CancellationToken cancellationToken = default);

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
    /// Загрузить файл и получить токен вложения
    /// </summary>
    /// <param name="request">Запрос на загрузку файла</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Токен загруженного файла</returns>
    Task<string> UploadsAsync(UploadRequest request, CancellationToken cancellationToken = default);

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
