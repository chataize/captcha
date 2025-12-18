using System.Diagnostics.CodeAnalysis;

namespace ChatAIze.Captcha;

/// <summary>
/// Configuration options for hCaptcha integration.
/// </summary>
public sealed record CaptchaOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CaptchaOptions"/> class.
    /// </summary>
    public CaptchaOptions() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CaptchaOptions"/> class with required values.
    /// </summary>
    /// <param name="siteKey">hCaptcha site key.</param>
    /// <param name="secret">hCaptcha secret key.</param>
    /// <param name="verifyIpAddresses">Whether to pass client IPs during verification.</param>
    /// <param name="isConnectionProxied">Whether to read client IPs from forwarded headers.</param>
    [SetsRequiredMembers]
    public CaptchaOptions(string siteKey, string secret, bool verifyIpAddresses = false, bool isConnectionProxied = false)
    {
        SiteKey = siteKey;
        Secret = secret;
        VerifyIpAddresses = verifyIpAddresses;
        IsConnectionProxied = isConnectionProxied;
    }

    /// <summary>
    /// Gets or sets the hCaptcha site key.
    /// </summary>
    public required string SiteKey { get; set; }

    /// <summary>
    /// Gets or sets the hCaptcha secret.
    /// </summary>
    public required string Secret { get; set; }

    /// <summary>
    /// Gets or sets whether to include the client IP in verification requests.
    /// </summary>
    public bool VerifyIpAddresses { get; set; }

    /// <summary>
    /// Gets or sets whether the app is running behind a proxy.
    /// </summary>
    public bool IsConnectionProxied { get; set; }
}
