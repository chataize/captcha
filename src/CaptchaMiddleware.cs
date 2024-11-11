using Microsoft.AspNetCore.Http;

namespace ChatAIze.Captcha;

public sealed class CaptchaMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, CaptchaService captchaService)
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
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
