using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace ChatAIze.Captcha;

[method: ActivatorUtilitiesConstructor]
public sealed class CaptchaService(IOptions<CaptchaOptions> options, IJSRuntime jsRuntime) : IAsyncDisposable
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
        services.AddScoped<CaptchaService>();
        services.Configure(configureOptions);

        return services;
    }
}
