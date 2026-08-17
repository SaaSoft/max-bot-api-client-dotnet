using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using MAX.Bot.Exceptions;
using MAX.Bot.Interfaces;
using MAX.Bot.Interfaces.Models;
using MAX.Bot.Interfaces.Models.Attachment;
using MAX.Bot.Interfaces.Models.Request;
using MAX.Bot.Interfaces.Models.Request.Message;
using MAX.Bot.Interfaces.Models.Request.Message.Attachment;
using MAX.Bot.Interfaces.Models.Response;
using static MAX.Bot.FrameworkSpecificMethods;

namespace MAX.Bot;

public class MaxBotClient : IMaxBotClient
{
    private static readonly JsonSerializerOptions DeserializeOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowOutOfOrderMetadataProperties = true,
    };

    private static readonly JsonSerializerOptions SerializeOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        AllowOutOfOrderMetadataProperties = true,
    };

    private static readonly HttpClient DownloadHttpClient = new();

    private readonly HttpClient _httpClient;
    private readonly AttachmentRetryOptions _attachmentRetryOptions;
    private readonly string _baseUrl = "https://platform-api2.max.ru";

    public CancellationToken GlobalCancelToken { get; }

    public AttachmentRetryOptions AttachmentRetryOptions => _attachmentRetryOptions;

    public MaxBotClient(
        string token,
        HttpClient httpClient,
        AttachmentRetryOptions? attachmentRetryOptions = null,
        CancellationToken cancellationToken = default)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        _httpClient.BaseAddress = new Uri(_baseUrl);

        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            _httpClient.DefaultRequestHeaders.Add("Authorization", token);

        _attachmentRetryOptions = attachmentRetryOptions ?? new AttachmentRetryOptions();
        GlobalCancelToken = cancellationToken;
    }

    public MaxBotClient(
        string token,
        int timeoutSeconds = 30,
        AttachmentRetryOptions? attachmentRetryOptions = null,
        CancellationToken cancellationToken = default)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_baseUrl),
            Timeout = TimeSpan.FromSeconds(timeoutSeconds)
        };

        _httpClient.DefaultRequestHeaders.Add("Authorization", token);

        _attachmentRetryOptions = attachmentRetryOptions ?? new AttachmentRetryOptions();
        GlobalCancelToken = cancellationToken;
    }

    private async Task<T> SendRequestAsync<T>(HttpMethod method, string endpoint, object? data = null,
        CancellationToken cancellationToken = default)
    {
        var requestUri = new Uri(_httpClient.BaseAddress!, endpoint);
        var request = new HttpRequestMessage(method, requestUri);

        if (data != null && (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod_Patch))
        {
            var json = JsonSerializer.Serialize(data, SerializeOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, GlobalCancelToken);
        var response = await _httpClient.SendAsync(request, cts.Token);
        var responseContent = await response.Content.ReadAsStringAsync(cts.Token);

        if (!response.IsSuccessStatusCode)
        {
            throw new MaxBotClientException(
                $"HTTP {response.StatusCode}: {responseContent}",
                response.StatusCode);
        }

        return JsonSerializer.Deserialize<T>(responseContent, DeserializeOptions)
            ?? throw new InvalidOperationException("Не удалось десериализовать ответ");
    }

    public async Task<User> GetMeAsync(CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<User>(
            HttpMethod.Get, "/me", null, cancellationToken);
    }

    public async Task<UpdateBotCommandsResponse> UpdateBotCommandsAsync(
        UpdateBotCommandsRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException_ThrowIfNull(request);
        request.Validate();

        return await SendRequestAsync<UpdateBotCommandsResponse>(
            HttpMethod_Patch,
            "/me/commands",
            request,
            cancellationToken);
    }

    public async Task<SendMessageResponse> SendMessageAsync(
        SendMessageRequest request,
        AttachmentRetryOptions? retryOptions = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();

        if (request.UserId.HasValue)
            queryParams.Add($"user_id={request.UserId.Value}");

        if (request.ChatId.HasValue)
            queryParams.Add($"chat_id={request.ChatId.Value}");

        if (request.DisableLinkPreview.HasValue)
            queryParams.Add($"disable_link_preview={request.DisableLinkPreview.Value.ToString().ToLower()}");

        var queryString = queryParams.Any() ? $"?{string.Join("&", queryParams)}" : "";

        var send = () => SendRequestAsync<SendMessageResponse>(
            HttpMethod.Post, $"/messages{queryString}", request, cancellationToken);

        if (RequiresAttachmentRetry(request))
        {
            return await RetryWhileAttachmentNotReadyAsync(
                send,
                retryOptions,
                cancellationToken);
        }

        return await send();
    }

    private static bool RequiresAttachmentRetry(SendMessageRequest request) =>
        request.Attachments?.Any(static a =>
            a is ImageAttachment or VideoAttachment or FileAttachment or AudioAttachment) == true;

    public async Task<BaseResponse> AnswerCallbackAsync(
        AnswerCallbackRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException_ThrowIfNull(request);
        request.Validate();

        return await SendRequestAsync<BaseResponse>(
            HttpMethod.Post,
            $"/answers?callback_id={HttpUtility_UrlEncode(request.CallbackId)}",
            request,
            cancellationToken);
    }

    public async Task<GetMessagesResponse> GetMessagesAsync(GetMessagesRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException_ThrowIfNull(request);
        request.Validate();

        var queryParams = new Dictionary<string, string>();

        if (request.ChatId.HasValue)
            queryParams["chat_id"] = request.ChatId.Value.ToString();

        if (request.MessageIds is { Count: > 0 })
            queryParams["message_ids"] = string.Join(",", request.MessageIds);

        if (request.Count.HasValue)
            queryParams["count"] = request.Count.Value.ToString();

        if (request.From.HasValue)
            queryParams["from"] = request.From.Value.ToString();

        if (request.To.HasValue)
            queryParams["to"] = request.To.Value.ToString();

        var queryString = queryParams.Any()
            ? "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={HttpUtility_UrlEncode(kvp.Value)}"))
            : "";

        return await SendRequestAsync<GetMessagesResponse>(HttpMethod.Get, $"/messages{queryString}", null, cancellationToken);
    }

    public async Task<Message> GetMessageByIdAsync(string messageId, CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<Message>(HttpMethod.Get, $"/messages/{messageId}", null, cancellationToken);
    }

    public async Task<VideoInfoResponse> GetVideoAsync(
        string videoToken,
        AttachmentRetryOptions? retryOptions = null,
        CancellationToken cancellationToken = default)
    {
        ValidateVideoToken(videoToken);

        return await RetryWhileAttachmentNotReadyAsync(
            () => GetVideoInfoAsync(videoToken, cancellationToken),
            retryOptions,
            cancellationToken);
    }

    private static void ValidateVideoToken(string videoToken)
    {
        if (string.IsNullOrWhiteSpace(videoToken))
            throw new ArgumentException("Токен видео-вложения не должен быть пустым.", nameof(videoToken));

        if (!IsVideoTokenValid(videoToken))
            throw new ArgumentException("Токен видео-вложения может содержать только латинские буквы, цифры, '_' и '-'.", nameof(videoToken));
    }

    private Task<VideoInfoResponse> GetVideoInfoAsync(string videoToken, CancellationToken cancellationToken) =>
        SendRequestAsync<VideoInfoResponse>(
            HttpMethod.Get,
            $"/videos/{Uri.EscapeDataString(videoToken)}",
            null,
            cancellationToken);

    public async Task<string?> TryGetAttachmentDownloadUrlAsync(
        Attachment attachment,
        VideoQuality videoQuality = VideoQuality.Mp4_480,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException_ThrowIfNull(attachment);

        try
        {
            return await GetAttachmentDownloadUrlAsync(attachment, videoQuality, cancellationToken);
        }
        catch (MaxBotClientException ex) when (ex.IsAttachmentNotReady)
        {
            return null;
        }
    }

    public async Task<AttachmentDownloadResult> DownloadAttachmentAsync(
        Attachment attachment,
        DownloadAttachmentOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException_ThrowIfNull(attachment);
        options ??= new DownloadAttachmentOptions();

        var maxAttempts = Math.Max(1, options.MaxAttempts);

        string? url = null;
        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            url = await TryGetAttachmentDownloadUrlAsync(attachment, options.VideoQuality, cancellationToken);
            if (!string.IsNullOrWhiteSpace(url))
                break;

            if (attempt < maxAttempts - 1)
                await Task.Delay(options.RetryDelay, cancellationToken);
        }

        if (string.IsNullOrWhiteSpace(url))
        {
            throw new MaxBotClientException(
                "Вложение ещё не готово к скачиванию или URL недоступен.",
                HttpStatusCode.ServiceUnavailable,
                isAttachmentNotReady: true);
        }

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, GlobalCancelToken);
#if NET462
        var content = await DownloadHttpClient.GetByteArrayAsync(url).ConfigureAwait(false);
#else
        var content = await DownloadHttpClient.GetByteArrayAsync(url, cts.Token);
#endif
        var fileName = ResolveDownloadFileName(options, url, attachment);

        if (string.IsNullOrWhiteSpace(options.FilePath))
        {
            return new AttachmentDownloadResult
            {
                Content = content,
                FileName = fileName,
            };
        }

        var savedFilePath = ResolveSavedFilePath(options.FilePath, url, attachment, options.DefaultExtension);
        var directory = Path.GetDirectoryName(savedFilePath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

#if NET462
        File.WriteAllBytes(savedFilePath, content);
#else
        await File.WriteAllBytesAsync(savedFilePath, content, cts.Token);
#endif

        return new AttachmentDownloadResult
        {
            SavedFilePath = savedFilePath,
            FileName = Path.GetFileName(savedFilePath),
        };
    }

    private static string ResolveDownloadFileName(
        DownloadAttachmentOptions options,
        string url,
        Attachment attachment)
    {
        var preferredFileName = attachment.GetFileName();
        if (!string.IsNullOrWhiteSpace(preferredFileName))
            return preferredFileName;

        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            var fileName = Path.GetFileName(uri.AbsolutePath);
            if (!string.IsNullOrWhiteSpace(fileName))
                return fileName;
        }

        var extension = options.DefaultExtension ?? GetDefaultExtension(attachment) ?? ".bin";
        if (!extension.StartsWith('.'))
            extension = "." + extension;

        return $"download{extension}";
    }

    private static string ResolveSavedFilePath(
        string filePath,
        string url,
        Attachment attachment,
        string? defaultExtension)
    {
        if (HasRecognizedFileExtension(filePath))
            return filePath;

        var extension = Path.GetExtension(attachment.GetFileName() ?? string.Empty);
        if (string.IsNullOrWhiteSpace(extension) && Uri.TryCreate(url, UriKind.Absolute, out var uri))
            extension = Path.GetExtension(uri.AbsolutePath);
        if (string.IsNullOrWhiteSpace(extension))
            extension = defaultExtension ?? GetDefaultExtension(attachment) ?? ".bin";
        if (!extension.StartsWith('.'))
            extension = "." + extension;

        return filePath + extension;
    }

    private static bool HasRecognizedFileExtension(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        if (string.IsNullOrEmpty(extension) || extension.Length < 2 || extension.Length > 11)
            return false;

        for (var i = 1; i < extension.Length; i++)
        {
            if (!char.IsLetterOrDigit(extension[i]))
                return false;
        }

        return true;
    }

    private static string? GetDefaultExtension(Attachment attachment) => attachment switch
    {
        ImageAttachment => ".jpg",
        VideoAttachment => ".mp4",
        FileAttachment => ".bin",
        _ => null,
    };

    private async Task<string?> GetAttachmentDownloadUrlAsync(
        Attachment attachment,
        VideoQuality videoQuality,
        CancellationToken cancellationToken)
    {
        switch (attachment)
        {
            case ImageAttachment image:
                return image.Payload.Url;
            case FileAttachment file:
                return file.Payload.Url;
            case VideoAttachment video:
                return (await GetVideoInfoAsync(video.Payload.Token, cancellationToken))
                    .TryGetDownloadUrl(videoQuality);
            default:
                throw new NotSupportedException(
                    $"Скачивание не поддерживается для вложения типа {attachment.GetType().Name}.");
        }
    }

    public async Task<BaseResponse> EditMessageByIdAsync(
        string messageId,
        SendMessageRequest messageRequest,
        AttachmentRetryOptions? retryOptions = null,
        CancellationToken cancellationToken = default)
    {
        var edit = () => SendRequestAsync<BaseResponse>(
            HttpMethod.Put, $"/messages?message_id={messageId}", messageRequest, cancellationToken);

        if (RequiresAttachmentRetry(messageRequest))
        {
            return await RetryWhileAttachmentNotReadyAsync(
                edit,
                retryOptions,
                cancellationToken);
        }

        return await edit();
    }

    public async Task<BaseResponse> DeleteMessageByIdAsync(string messageId, CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<BaseResponse>(HttpMethod.Delete, $"/messages?message_id={messageId}", null, cancellationToken);
    }

    public async Task<GetChatsResponse> GetChatsAsync(GetChatsRequest request, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string>();

        if (request.Count.HasValue)
            queryParams["count"] = request.Count.Value.ToString();

        if (request.Marker.HasValue)
            queryParams["marker"] = request.Marker.Value.ToString();

        var queryString = queryParams.Any()
            ? "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={HttpUtility_UrlEncode(kvp.Value)}"))
            : "";

        return await SendRequestAsync<GetChatsResponse>(HttpMethod.Get, $"/chats{queryString}", null, cancellationToken);
    }

    public async Task<Chat> GetChatByIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<Chat>(
            HttpMethod.Get,
            $"/chats/{chatId.ToString(CultureInfo.InvariantCulture)}",
            null,
            cancellationToken);
    }

    public async Task<Chat> UpdateChatAsync(long chatId, UpdateChatRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException_ThrowIfNull(request);
        request.Validate();

        return await SendRequestAsync<Chat>(
            HttpMethod_Patch,
            $"/chats/{chatId.ToString(CultureInfo.InvariantCulture)}",
            request,
            cancellationToken);
    }

    public async Task<BaseResponse> DeleteChatAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<BaseResponse>(
            HttpMethod.Delete,
            $"/chats/{chatId.ToString(CultureInfo.InvariantCulture)}",
            null,
            cancellationToken);
    }

    public async Task<BaseResponse> SendChatActionAsync(
        long chatId,
        SendChatActionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException_ThrowIfNull(request);
        request.Validate();

        return await SendRequestAsync<BaseResponse>(
            HttpMethod.Post,
            $"/chats/{chatId.ToString(CultureInfo.InvariantCulture)}/actions",
            request,
            cancellationToken);
    }

    public async Task<GetChatPinnedMessageResponse> GetChatPinnedMessageAsync(
        long chatId,
        CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<GetChatPinnedMessageResponse>(
            HttpMethod.Get,
            $"/chats/{chatId.ToString(CultureInfo.InvariantCulture)}/pin",
            null,
            cancellationToken);
    }

    public async Task<BaseResponse> PinChatMessageAsync(
        long chatId,
        PinChatMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException_ThrowIfNull(request);
        request.Validate();

        return await SendRequestAsync<BaseResponse>(
            HttpMethod.Put,
            $"/chats/{chatId.ToString(CultureInfo.InvariantCulture)}/pin",
            request,
            cancellationToken);
    }

    public async Task<BaseResponse> UnpinChatMessageAsync(
        long chatId,
        CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<BaseResponse>(
            HttpMethod.Delete,
            $"/chats/{chatId.ToString(CultureInfo.InvariantCulture)}/pin",
            null,
            cancellationToken);
    }

    public async Task<ChatMember> GetChatMembershipAsync(
        long chatId,
        CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<ChatMember>(
            HttpMethod.Get,
            $"/chats/{chatId.ToString(CultureInfo.InvariantCulture)}/members/me",
            null,
            cancellationToken);
    }

    public async Task<BaseResponse> LeaveChatAsync(
        long chatId,
        CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<BaseResponse>(
            HttpMethod.Delete,
            $"/chats/{chatId.ToString(CultureInfo.InvariantCulture)}/members/me",
            null,
            cancellationToken);
    }

    public async Task<GetChatMembersResponse> GetChatAdminsAsync(
        long chatId,
        CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<GetChatMembersResponse>(
            HttpMethod.Get,
            $"/chats/{chatId.ToString(CultureInfo.InvariantCulture)}/members/admins",
            null,
            cancellationToken);
    }

    public async Task<BaseResponse> AddChatAdminsAsync(
        long chatId,
        AddChatAdminsRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException_ThrowIfNull(request);
        request.Validate();

        return await SendRequestAsync<BaseResponse>(
            HttpMethod.Post,
            $"/chats/{chatId.ToString(CultureInfo.InvariantCulture)}/members/admins",
            request,
            cancellationToken);
    }

    public async Task<BaseResponse> RemoveChatAdminAsync(
        long chatId,
        long userId,
        CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
            throw new ArgumentException("ID пользователя должен быть положительным числом.", nameof(userId));

        return await SendRequestAsync<BaseResponse>(
            HttpMethod.Delete,
            $"/chats/{chatId.ToString(CultureInfo.InvariantCulture)}/members/admins/{userId.ToString(CultureInfo.InvariantCulture)}",
            null,
            cancellationToken);
    }

    public async Task<GetChatMembersResponse> GetChatMembersAsync(GetChatMembersRequest request, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string>();

        if (request.UserIds != null)
            queryParams["message_ids"] = string.Join(",", request.UserIds!);

        if (request.Marker.HasValue)
            queryParams["marker"] = request.Marker.Value.ToString();

        if (request.Count.HasValue)
            queryParams["count"] = request.Count.Value.ToString();

        var queryString = queryParams.Any()
            ? "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={HttpUtility_UrlEncode(kvp.Value)}"))
            : "";

        return await SendRequestAsync<GetChatMembersResponse>(
            HttpMethod.Get,
            $"/chats/{request.ChatId.ToString()}/members{queryString}",
            null,
            cancellationToken
        );
    }

    public async Task<BaseResponse> DeleteChatMemberAsync(DeleteChatMemberRequest request, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string>();

        if (request?.UserId != null)
            queryParams["user_id"] = request.UserId.ToString();

        if (request?.Block != null && request.Block.HasValue)
            queryParams["block"] = request.Block.Value.ToString();

        var queryString = queryParams.Any()
            ? "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={HttpUtility_UrlEncode(kvp.Value)}"))
            : "";

        return await SendRequestAsync<BaseResponse>(
            HttpMethod.Delete,
            $"/chats/{request?.ChatId.ToString()}/members{queryString}",
            null,
            cancellationToken
        );
    }

    public async Task<BaseResponse> AddChatMemberAsync(AddChatMemberRequest request, CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<BaseResponse>(
            HttpMethod.Post,
            $"/chats/{request.ChatId.ToString()}/members",
            request,
            cancellationToken
        );
    }

    public async Task<string> UploadsAsync(UploadRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException_ThrowIfNull(request);
        request.Validate();

        var uploadType = request.GetUploadTypeValue();
        var uploadInfo = await SendRequestAsync<UploadResponse>(
            HttpMethod.Post,
            $"/uploads?type={HttpUtility_UrlEncode(uploadType)}",
            null,
            cancellationToken);

        if (string.IsNullOrWhiteSpace(uploadInfo.Url))
            throw new InvalidOperationException("API MAX вернул пустой URL для загрузки файла");

        var uploadUri = CreateUploadUri(uploadInfo.Url);
        var uploadResponse = await UploadFileAsync(uploadUri, request, cancellationToken);
        return ResolveUploadToken(request, uploadInfo, uploadResponse);
    }

    public async Task<GetUpdatesResponse> GetUpdatesAsync(
        GetUpdatesRequest request,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string>();

        if (request.Limit.HasValue)
            queryParams["limit"] = request.Limit.Value.ToString();

        if (request.Timeout.HasValue)
            queryParams["timeout"] = request.Timeout.Value.ToString();

        if (request.Marker.HasValue)
            queryParams["marker"] = request.Marker.Value.ToString();

        if (request.Types != null && request.Types.Count != 0)
            queryParams["types"] = string.Join(",", request.Types);

        var queryString = queryParams.Any()
            ? "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={HttpUtility_UrlEncode(kvp.Value)}"))
            : "";

        return await SendRequestAsync<GetUpdatesResponse>(
            HttpMethod.Get,
            $"/updates{queryString}",
            null,
            cancellationToken);
    }

    public async Task<GetSubscriptionsResponse> GetSubscriptionsAsync(CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<GetSubscriptionsResponse>(
            HttpMethod.Get,
            "/subscriptions",
            null,
            cancellationToken);
    }

    public async Task<BaseResponse> SubscribeAsync(
        SubscriptionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException_ThrowIfNull(request);
        request.Validate();

        return await SendRequestAsync<BaseResponse>(
            HttpMethod.Post,
            "/subscriptions",
            request,
            cancellationToken);
    }

    public async Task<BaseResponse> UnsubscribeAsync(
        DeleteSubscriptionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException_ThrowIfNull(request);
        request.Validate();

        return await SendRequestAsync<BaseResponse>(
            HttpMethod.Delete,
            $"/subscriptions?url={HttpUtility_UrlEncode(request.Url)}",
            null,
            cancellationToken);
    }

    public async Task PollUpdatesWithCallback(
        Func<Update, IMaxBotClient, Task> callback,
        int? limit = 100,
        int? timeout = 30,
        long? marker = null,
        List<string>? types = null,
        CancellationToken cancellationToken = default)
    {
        long? currentMarker = marker;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var response = await GetUpdatesAsync(new GetUpdatesRequest
                {
                    Limit = limit,
                    Timeout = timeout,
                    Marker = currentMarker,
                    Types = types
                }, cancellationToken);

                foreach (var update in response.Updates)
                {
                    await callback.Invoke(update, this);
                }

                currentMarker = response.Marker;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    private async Task<UploadResponse> UploadFileAsync(
        Uri uploadUri,
        UploadRequest request,
        CancellationToken cancellationToken)
    {
        using var fileContent = new StreamContent(new NonDisposingStream(request.Content!));

        if (!string.IsNullOrWhiteSpace(request.ContentType))
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(request.ContentType);

        using var multipartContent = new MultipartFormDataContent();
        multipartContent.Add(fileContent, "data", request.FileName);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, GlobalCancelToken);
        var response = await _httpClient.PostAsync(uploadUri, multipartContent, cts.Token);
        var responseContent = await response.Content.ReadAsStringAsync(cts.Token);

        if (!response.IsSuccessStatusCode)
        {
            throw new MaxBotClientException(
                $"HTTP {response.StatusCode}: {responseContent}",
                response.StatusCode);
        }

        if (string.IsNullOrWhiteSpace(responseContent))
            return new UploadResponse();

        if (!IsJsonResponse(responseContent))
        {
            if (request.Type is UploadType.Video or UploadType.Audio)
                return new UploadResponse();

            throw new InvalidOperationException(
                $"API MAX вернул не JSON-ответ после загрузки файла: {CreateResponsePreview(responseContent)}");
        }

        return JsonSerializer.Deserialize<UploadResponse>(responseContent, DeserializeOptions)
            ?? throw new InvalidOperationException("Не удалось десериализовать ответ загрузки файла");
    }

    private static Uri CreateUploadUri(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uploadUri) ||
            (uploadUri.Scheme != Uri.UriSchemeHttps && uploadUri.Scheme != Uri.UriSchemeHttp))
        {
            throw new InvalidOperationException("API MAX вернул некорректный URL для загрузки файла");
        }

        return uploadUri;
    }

    private static string ResolveUploadToken(
        UploadRequest request,
        UploadResponse uploadInfo,
        UploadResponse uploadResponse)
    {
        var token = request.Type switch
        {
            UploadType.Video or UploadType.Audio => uploadInfo.Token ?? uploadResponse.Token,
			UploadType.File => uploadResponse.Token ?? uploadInfo.Token,
			UploadType.Image => uploadResponse.Photos?.Values?.FirstOrDefault()?.Token ?? uploadInfo.Token,
			_ => null
        };

        if (!string.IsNullOrWhiteSpace(token))
            return token!;

        var expectedStep = request.Type is UploadType.Video or UploadType.Audio
            ? "первого шага загрузки video/audio"
            : "ответа загрузки image/file";

        throw new InvalidOperationException($"API MAX не вернул token из {expectedStep}.");
    }

    private static bool IsJsonResponse(string responseContent)
    {
        var trimmed = responseContent.TrimStart();
        return trimmed.StartsWith('{') || trimmed.StartsWith('[');
    }

    private async Task<T> RetryWhileAttachmentNotReadyAsync<T>(
        Func<Task<T>> action,
        AttachmentRetryOptions? retryOptions,
        CancellationToken cancellationToken)
    {
        var options = retryOptions ?? _attachmentRetryOptions;
        var maxAttempts = Math.Max(1, options.MaxAttempts);

        for (var attempt = 0; ; attempt++)
        {
            try
            {
                return await action();
            }
            catch (MaxBotClientException ex) when (ex.IsAttachmentNotReady && attempt < maxAttempts - 1)
            {
                await Task.Delay(options.RetryDelay, cancellationToken);
            }
        }
    }

    private static bool IsVideoTokenValid(string videoToken)
    {
        return videoToken.All(c => Char_IsAsciiLetterOrDigit(c) || c is '_' or '-');
    }

    private static string CreateResponsePreview(string responseContent)
    {
        const int maxLength = 200;

        var normalized = responseContent
            .Replace("\r", string.Empty)
            .Replace("\n", " ");

        return normalized.Length <= maxLength
            ? normalized
            : normalized[..maxLength] + "...";
    }

    private sealed class NonDisposingStream : Stream
    {
        private readonly Stream _inner;

        public NonDisposingStream(Stream inner)
        {
            _inner = inner;
        }

        public override bool CanRead => _inner.CanRead;
        public override bool CanSeek => _inner.CanSeek;
        public override bool CanWrite => _inner.CanWrite;
        public override long Length => _inner.Length;

        public override long Position
        {
            get => _inner.Position;
            set => _inner.Position = value;
        }

        public override void Flush()
        {
            _inner.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _inner.FlushAsync(cancellationToken);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _inner.Read(buffer, offset, count);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return base.ReadAsync(buffer, offset, count, cancellationToken);
        }

#if !NET462
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return _inner.ReadAsync(buffer, cancellationToken);
        }
#endif

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _inner.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _inner.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _inner.Write(buffer, offset, count);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return base.WriteAsync(buffer, offset, count, cancellationToken);
        }

#if !NET462
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return _inner.WriteAsync(buffer, cancellationToken);
        }
#endif

        protected override void Dispose(bool disposing)
        {
        }
    }
}
