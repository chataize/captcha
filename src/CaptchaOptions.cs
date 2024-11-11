using System.Diagnostics.CodeAnalysis;

namespace ChatAIze.Captcha;

public sealed record CaptchaOptions
{
    public CaptchaOptions() { }

    [SetsRequiredMembers]
    public CaptchaOptions(string siteKey, string secret, bool verifyIpAddresses = true, bool isConnectionProxied = false)
    {
        SiteKey = siteKey;
        Secret = secret;
        VerifyIpAddresses = verifyIpAddresses;
        IsConnectionProxied = isConnectionProxied;
    }

    public required string SiteKey { get; set; }

    public required string Secret { get; set; }

    public bool VerifyIpAddresses { get; set; } = true;

    public bool IsConnectionProxied { get; set; }
}
