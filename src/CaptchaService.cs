using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace ChatAIze.Captcha;

[method: ActivatorUtilitiesConstructor]
public sealed class CaptchaService(HttpClient httpClient, IJSRuntime jsRuntime, IOptions<CaptchaOptions> options) : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/ChatAIze.Captcha/captcha.js").AsTask());

    internal string? IpAddress { get; set; }

    public async Task InitializeAsync(int id, DotNetObjectReference<Captcha> reference, CaptchaTheme theme, CaptchaSize size)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("initCaptcha", id, reference, options.Value.SiteKey, theme.ToString().ToLowerInvariant(), size.ToString().ToLowerInvariant());
    }

    public async Task<bool> VerifyTokenAsync(string token, string? ipAddress)
    {
        var values = new Dictionary<string, string>
        {
            ["secret"] = options.Value.Secret,
            ["sitekey"] = options.Value.SiteKey,
            ["response"] = token,
        };

        if (options.Value.VerifyIpAddresses && !string.IsNullOrWhiteSpace(ipAddress) && ipAddress != "::1")
        {
            values["remoteip"] = ipAddress;
        }

        var requestContent = new FormUrlEncodedContent(values);
        var response = await httpClient.PostAsync("https://hcaptcha.com/siteverify", requestContent);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var responseContent = await response.Content.ReadAsStreamAsync();
        var responseDocument = await JsonDocument.ParseAsync(responseContent);

        return responseDocument.RootElement.GetProperty("success").GetBoolean();
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
