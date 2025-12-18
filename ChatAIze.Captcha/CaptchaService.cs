using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace ChatAIze.Captcha;

[method: ActivatorUtilitiesConstructor]
internal sealed class CaptchaService(HttpClient httpClient, IJSRuntime jsRuntime, IOptions<CaptchaOptions> options) : IAsyncDisposable
{
    // Lazy-load the JS module so we only import it when a component actually renders.
    private readonly Lazy<Task<IJSObjectReference>> moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/ChatAIze.Captcha/captcha.js").AsTask());

    // Captured per request by middleware; used when validating the token.
    internal string? IpAddress { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }

    internal async Task InitializeAsync(int id, DotNetObjectReference<Captcha> reference, string? siteKey, CaptchaTheme theme, CaptchaSize size)
    {
        var module = await moduleTask.Value;
        // Default to configured site key when the component does not supply one.
        await module.InvokeVoidAsync("initCaptcha", id, reference, siteKey ?? options.Value.SiteKey, theme.ToString().ToLowerInvariant(), size.ToString().ToLowerInvariant());
    }

    internal async Task<bool> VerifyTokenAsync(string token, string? ipAddress, string? siteKey, string? secret)
    {
        try
        {
            // hCaptcha verification API expects form-urlencoded values.
            var values = new Dictionary<string, string>
            {
                ["secret"] = secret ?? options.Value.Secret,
                ["sitekey"] = siteKey ?? options.Value.SiteKey,
                ["response"] = token,
            };

            // Only include a client IP when configured and avoid loopback in local dev.
            if (options.Value.VerifyIpAddresses && !string.IsNullOrWhiteSpace(ipAddress) && ipAddress != "::1")
            {
                values["remoteip"] = ipAddress;
            }

            var requestContent = new FormUrlEncodedContent(values);
            var response = await httpClient.PostAsync("https://hcaptcha.com/siteverify", requestContent);

            // Treat non-2xx responses as verification failures.
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var responseContent = await response.Content.ReadAsStreamAsync();
            var responseDocument = await JsonDocument.ParseAsync(responseContent);

            // Parse the success flag from the JSON response.
            return responseDocument.RootElement.GetProperty("success").GetBoolean();
        }
        catch
        {
            // Be conservative: any exception counts as a failed verification.
            return false;
        }
    }
}
