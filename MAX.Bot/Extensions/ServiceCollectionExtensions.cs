using MAX.Bot.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MAX.Bot.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMaxBotClient(
       this IServiceCollection services,
       string token,
       int timeoutSeconds = 30)
    {
        services.AddHttpClient("MaxBot", client =>
        {
            client.BaseAddress = new Uri("https://platform-api.max.ru");
            client.DefaultRequestHeaders.Add("Authorization", token);
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        });

        services.AddScoped<IMaxBotClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("MaxBot");

            return new MaxBotClient(token, httpClient);
        });

        return services;
    }
}