using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ChatAIze.Captcha;

/// <summary>
/// Captures client IPs for hCaptcha verification when enabled.
/// </summary>
/// <param name="next">Next middleware in the pipeline.</param>
internal sealed class CaptchaMiddleware(RequestDelegate next)
{
    /// <summary>
    /// Executes the middleware and stores the client IP for later verification.
    /// </summary>
    /// <param name="context">Current HTTP context.</param>
    /// <param name="captchaService">Captcha service that stores the IP.</param>
    /// <param name="options">Captcha options that control IP capture.</param>
    /// <returns>A task that completes after downstream middleware runs.</returns>
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
