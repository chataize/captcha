using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace ChatAIze.Captcha;

public sealed class CaptchaService(IJSRuntime jsRuntime) : IAsyncDisposable
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
    public static IServiceCollection AddCaptchaService(this IServiceCollection services)
    {
        services.AddScoped<CaptchaService>();
        return services;
    }
}
