using MAX.Bot.Exceptions;
using MAX.Bot.Interfaces;
using System.Text;
using System.Text.Json;
using System.Web;

namespace MAX.Bot;

public class MaxBotClient : IMaxBotClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://platform-api.max.ru";

    public CancellationToken GlobalCancelToken { get; }

    public MaxBotClient(string token, HttpClient httpClient, CancellationToken cancellationToken = default)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        _httpClient.BaseAddress = new Uri(_baseUrl);

        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            _httpClient.DefaultRequestHeaders.Add("Authorization", token);

        GlobalCancelToken = cancellationToken;
    }

    public MaxBotClient(string token, int timeoutSeconds = 30, CancellationToken cancellationToken = default)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_baseUrl),
            Timeout = TimeSpan.FromSeconds(timeoutSeconds)
        };

        _httpClient.DefaultRequestHeaders.Add("Authorization", token);

        GlobalCancelToken = cancellationToken;
    }

    private async Task<T> SendRequestAsync<T>(HttpMethod method, string endpoint, object? data = null,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(endpoint);
        var requestUri = new Uri(_httpClient.BaseAddress!, endpoint);
        var request = new HttpRequestMessage(method, requestUri);

        if (data != null && (method == HttpMethod.Post || method == HttpMethod.Put))
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
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

        return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Не удалось десериализовать ответ");
    }

    public async Task<User> GetMeAsync(CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<User>(
            HttpMethod.Get, "/me", null, cancellationToken);
    }

    public async Task<SendMessageResponse> SendMessageAsync(SendMessageRequest request, CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();

        if (request.UserId.HasValue)
            queryParams.Add($"user_id={request.UserId.Value}");

        if (request.ChatId.HasValue)
            queryParams.Add($"chat_id={request.ChatId.Value}");

        if (request.DisableLinkPreview.HasValue)
            queryParams.Add($"disable_link_preview={request.DisableLinkPreview.Value.ToString().ToLower()}");

        var queryString = queryParams.Any() ? $"?{string.Join("&", queryParams)}" : "";

        return await SendRequestAsync<SendMessageResponse>(
            HttpMethod.Post, $"/messages{queryString}", request, cancellationToken);
    }

    public async Task<GetMessagesResponse> GetMessagesAsync(GetMessagesRequest request, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string>();

        if (request.ChatId.HasValue)
            queryParams["chat_id"] = request.ChatId.Value.ToString();

        if (request.MessageIds != null)
            queryParams["message_ids"] = string.Join(",", request.MessageIds!);

        if (request.Count.HasValue)
            queryParams["count"] = request.Count.Value.ToString();

        if (request.From.HasValue)
            queryParams["from"] = request.From.Value.ToString();

        if (request.To.HasValue)
            queryParams["to"] = request.To.Value.ToString();

        var queryString = queryParams.Any()
            ? "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"))
            : "";

        return await SendRequestAsync<GetMessagesResponse>(HttpMethod.Get, $"/messages{queryString}", null, cancellationToken);
    }

    public async Task<Message> GetMessageByIdAsync(string messageId, CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<Message>(HttpMethod.Get, $"/messages/{messageId}", null, cancellationToken);
    }

    public async Task<GetChatsResponse> GetChatsAsync(GetChatsRequest request, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string>();

        if (request.Count.HasValue)
            queryParams["count"] = request.Count.Value.ToString();

        if (request.Marker.HasValue)
            queryParams["marker"] = request.Marker.Value.ToString();

        var queryString = queryParams.Any()
            ? "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"))
            : "";

        return await SendRequestAsync<GetChatsResponse>(HttpMethod.Get, $"/chats{queryString}", null, cancellationToken);
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
            ? "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"))
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
            ? "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"))
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
            ? "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"))
            : "";

        return await SendRequestAsync<GetUpdatesResponse>(
            HttpMethod.Get,
            $"/updates{queryString}",
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
}