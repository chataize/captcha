using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ChatAIze.Captcha;

internal sealed class CaptchaMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, CaptchaService captchaService, IOptions<CaptchaOptions> options)
    {
        // Skip IP capture if verification does not require it.
        if (!options.Value.VerifyIpAddresses)
        {
            await next(context);
            return;
        }

        // Prefer the forwarded header when running behind a proxy/load balancer.
        if (options.Value.IsConnectionProxied && context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            // Header may contain multiple IPs; pass through as-is for upstream parsing.
            captchaService.IpAddress = forwardedFor;
        }
        else
        {
            // Fall back to the direct connection IP.
            captchaService.IpAddress = context.Connection.RemoteIpAddress?.ToString();
        }

        await next(context);
    }
}
