using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ChatAIze.Captcha;

public static class CaptchaExtension
{
    public static IServiceCollection AddCaptcha(this IServiceCollection services, Action<CaptchaOptions>? configure = null)
    {
        services.AddHttpClient<CaptchaService>();
        services.AddScoped<CaptchaService>();

        if (configure is not null)
        {
            services.Configure(configure);
        }

        return services;
    }

    public static IApplicationBuilder UseCaptcha(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CaptchaMiddleware>();
    }
}
