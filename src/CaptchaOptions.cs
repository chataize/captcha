using System.Diagnostics.CodeAnalysis;

namespace ChatAIze.Captcha;

public sealed record CaptchaOptions
{
    public CaptchaOptions() { }

    [SetsRequiredMembers]
    public CaptchaOptions(string siteKey, string secret)
    {
        SiteKey = siteKey;
        Secret = secret;
    }

    public required string SiteKey { get; init; }

    public required string Secret { get; init; }
}
