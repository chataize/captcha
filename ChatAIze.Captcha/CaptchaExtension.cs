using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ChatAIze.Captcha;

/// <summary>
/// Extension methods for registering and configuring captcha services.
/// </summary>
public static class CaptchaExtension
{
    /// <summary>
    /// Registers the captcha services and optional configuration.
    /// </summary>
    /// <param name="services">Service collection to register with.</param>
    /// <param name="configure">Optional configuration delegate.</param>
    /// <returns>The updated service collection.</returns>
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

    /// <summary>
    /// Adds the captcha middleware to capture client IPs when needed.
    /// </summary>
    /// <param name="app">Application builder to configure.</param>
    /// <returns>The updated application builder.</returns>
    public static IApplicationBuilder UseCaptcha(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CaptchaMiddleware>();
    }
}
