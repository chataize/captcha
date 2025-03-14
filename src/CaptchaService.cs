using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace ChatAIze.Captcha;

[method: ActivatorUtilitiesConstructor]
internal sealed class CaptchaService(HttpClient httpClient, IJSRuntime jsRuntime, IOptions<CaptchaOptions> options) : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/ChatAIze.Captcha/captcha.js").AsTask());

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
        await module.InvokeVoidAsync("initCaptcha", id, reference, siteKey ?? options.Value.SiteKey, theme.ToString().ToLowerInvariant(), size.ToString().ToLowerInvariant());
    }

    internal async Task<bool> VerifyTokenAsync(string token, string? ipAddress, string? siteKey, string? secret)
    {
        try
        {
            var values = new Dictionary<string, string>
            {
                ["secret"] = secret ?? options.Value.Secret,
                ["sitekey"] = siteKey ?? options.Value.SiteKey,
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
        catch
        {
            return false;
        }
    }
}
