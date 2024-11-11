using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ChatAIze.Captcha;

public sealed class CaptchaMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, CaptchaService captchaService, IOptions<CaptchaOptions> options)
    {
        if (options.Value.IsConnectionProxied && context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            captchaService.IpAddress = forwardedFor;
        }
        else
        {
            captchaService.IpAddress = context.Connection.RemoteIpAddress?.ToString();
        }

        await next(context);
    }
}
