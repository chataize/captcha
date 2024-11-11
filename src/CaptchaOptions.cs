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

    public required string SiteKey { get; set; }

    public required string Secret { get; set; }
}
