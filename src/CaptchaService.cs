using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace ChatAIze.Captcha;

[method: ActivatorUtilitiesConstructor]
public sealed class CaptchaService(HttpClient httpClient, IJSRuntime jsRuntime, IOptions<CaptchaOptions> options) : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/ChatAIze.Captcha/captcha.js").AsTask());

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}

public static class CaptchaServiceExtensions
{
    public static IServiceCollection AddCaptchaService(this IServiceCollection services, Action<CaptchaOptions> configureOptions)
    {
        services.AddHttpClient<CaptchaService>();
        services.AddScoped<CaptchaService>();
        services.Configure(configureOptions);

        return services;
    }
}
