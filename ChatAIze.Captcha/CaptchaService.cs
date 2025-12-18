using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace ChatAIze.Captcha;

/// <summary>
/// Provides JS interop initialization and server-side verification for hCaptcha.
/// </summary>
/// <param name="httpClient">HTTP client used to call the hCaptcha verification API.</param>
/// <param name="jsRuntime">JS runtime used to load and invoke the widget module.</param>
/// <param name="options">Configured captcha options.</param>
[method: ActivatorUtilitiesConstructor]
internal sealed class CaptchaService(HttpClient httpClient, IJSRuntime jsRuntime, IOptions<CaptchaOptions> options) : IAsyncDisposable
{
    /// <summary>
    /// Lazy-load the JS module so we only import it when a component actually renders.
    /// </summary>
    private readonly Lazy<Task<IJSObjectReference>> moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/ChatAIze.Captcha/captcha.js").AsTask());

    /// <summary>
    /// Gets or sets the client IP captured per request by middleware.
    /// </summary>
    internal string? IpAddress { get; set; }

    /// <summary>
    /// Disposes the JS module if it has been initialized.
    /// </summary>
    /// <returns>A task that completes after the module is disposed.</returns>
    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }

    /// <summary>
    /// Initializes the hCaptcha widget via JS interop.
    /// </summary>
    /// <param name="id">DOM element ID that hosts the widget.</param>
    /// <param name="reference">.NET object reference for JS callbacks.</param>
    /// <param name="siteKey">Optional site key override.</param>
    /// <param name="theme">Widget theme selection.</param>
    /// <param name="size">Widget size selection.</param>
    /// <returns>A task that completes after the widget is initialized.</returns>
    internal async Task InitializeAsync(int id, DotNetObjectReference<Captcha> reference, string? siteKey, CaptchaTheme theme, CaptchaSize size)
    {
        var module = await moduleTask.Value;
        // Default to configured site key when the component does not supply one.
        await module.InvokeVoidAsync("initCaptcha", id, reference, siteKey ?? options.Value.SiteKey, theme.ToString().ToLowerInvariant(), size.ToString().ToLowerInvariant());
    }

    /// <summary>
    /// Verifies a client token with the hCaptcha API.
    /// </summary>
    /// <param name="token">Token returned by the widget.</param>
    /// <param name="ipAddress">Optional client IP to pass through.</param>
    /// <param name="siteKey">Optional site key override.</param>
    /// <param name="secret">Optional secret override.</param>
    /// <returns><c>true</c> when verification succeeds; otherwise <c>false</c>.</returns>
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
