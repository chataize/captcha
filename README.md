# CAPTCHA
C# .NET 10.0 Blazor Server component for [hCaptcha](https://www.hcaptcha.com).

> [!WARNING]
> This library is designed for Blazor Server with interactive rendering enabled. It does **NOT** support Blazor WebAssembly.

## Requirements
- .NET 10.0
- Blazor Server with interactive rendering (`@rendermode InteractiveServer`)
- hCaptcha site key and secret
- Client access to `https://js.hcaptcha.com/1/api.js`
- Server access to `https://hcaptcha.com/siteverify`

## Installation
### NuGet Package
#### NuGet CLI
```bash
dotnet add package ChatAIze.Captcha
```
#### Package Manager Console
```powershell
Install-Package ChatAIze.Captcha
```

## Quickstart
```text
// Program.cs
builder.Services.AddCaptcha(o =>
{
    o.SiteKey = builder.Configuration["Captcha:SiteKey"]!;
    o.Secret = builder.Configuration["Captcha:Secret"]!;
});

app.UseCaptcha();

// App.razor (or index.html)
<script src="https://js.hcaptcha.com/1/api.js" async defer></script>

// Any .razor component
@rendermode InteractiveServer
<Captcha @bind-IsVerified="_isVerified" />

@code {
    private bool _isVerified;
}
```

## Setup
### Services (Program.cs)
```cs
builder.Services.AddCaptcha(o =>
{
    o.SiteKey = builder.Configuration["Captcha:SiteKey"]!;
    o.Secret = builder.Configuration["Captcha:Secret"]!;
    o.VerifyIpAddresses = false; // optional, default is false
    o.IsConnectionProxied = false; // optional, default is false
});
```

> [!NOTE]
> If users connect through a reverse proxy (Cloudflare, Nginx, etc.), set `IsConnectionProxied = true`
> and ensure your proxy is configured to send a trusted `X-Forwarded-For` header.

### Configuration (appsettings.json + environment variables)
```json
{
  "Captcha": {
    "SiteKey": "YOUR_SITE_KEY",
    "Secret": "YOUR_SECRET"
  }
}
```
```bash
# Prefer environment variables for secrets in production
Captcha__SiteKey=YOUR_SITE_KEY
Captcha__Secret=YOUR_SECRET
```

### Middleware (Program.cs)
```cs
app.UseCaptcha();
```

> [!NOTE]
> If you use `UseForwardedHeaders`, call it before `UseCaptcha()` so the IP is already resolved.
> ```cs
> app.UseForwardedHeaders();
> app.UseCaptcha();
> ```
> Docs: https://learn.microsoft.com/aspnet/core/host-and-deploy/proxy-load-balancer

### JavaScript (App.razor or index.html)
```html
<head>
  <!-- ... -->
  <script src="https://js.hcaptcha.com/1/api.js" async defer></script>
  <!-- ... -->
</head>
```

### Using (_Imports.razor)
```razor
@using ChatAIze.Captcha
```

## Usage Guide
### Basic usage
```razor
@rendermode InteractiveServer

<Captcha @bind-IsVerified="_isVerified" />
<p>Verified: @_isVerified</p>

@code {
    private bool _isVerified;
}
```

### Handling success and errors
```razor
<Captcha
    @bind-IsVerified="_isVerified"
    Succeeded="OnSucceeded"
    Expired="OnExpired"
    Error="OnError" />

@code {
    private bool _isVerified;

    private void OnSucceeded() { }
    private void OnExpired() { }
    private void OnError(string code) { }
}
```

### Per-component overrides
```razor
<Captcha
    @bind-IsVerified="_isVerified"
    SiteKey="YOUR_SITE_KEY"
    Secret="YOUR_SECRET" />
```

### Theme and size
```razor
<Captcha
    @bind-IsVerified="_isVerified"
    Theme="CaptchaTheme.Auto"
    Size="CaptchaSize.Normal" />
```

### IP verification (optional)
```cs
builder.Services.AddCaptcha(o =>
{
    o.SiteKey = "...";
    o.Secret = "...";
    o.VerifyIpAddresses = true;
    o.IsConnectionProxied = true; // set to true if behind a proxy
});
```

> [!WARNING]
> Only enable IP verification if you understand the privacy implications and your proxy setup is trusted.

## Security & Privacy
- Never expose `Secret` to the client; it must stay server-side.
- Treat IP verification as personal data processing where applicable.
- If you use a proxy, ensure `X-Forwarded-For` is trusted and not spoofable.
- Re-check `IsVerified` on the server before sensitive operations.

## CSP / Network Allowlist
- Ensure CSP allows `https://js.hcaptcha.com` and hCaptcha-related endpoints.
- hCaptcha CSP guidance: https://docs.hcaptcha.com/configuration#content-security-policy-csp

## Options Reference
`CaptchaOptions`
- `SiteKey` (required): hCaptcha site key.
- `Secret` (required): hCaptcha secret key.
- `VerifyIpAddresses` (optional): Include client IPs in verification requests.
- `IsConnectionProxied` (optional): Read IPs from `X-Forwarded-For` when behind a proxy.

## Component Parameters
`Captcha`
- `IpAddress`: Optional IP override for verification.
- `SiteKey`: Optional site key override.
- `Secret`: Optional secret override.
- `Theme`: `Auto`, `Light`, `Dark`.
- `Size`: `Normal`, `Compact`.
- `IsVerified`: Two-way bound verification state.
- Events: `Opened`, `Closed`, `Succeeded`, `Expired`, `ChallengeExpired`, `Error`.

## Best Practices
- Gate sensitive actions on `IsVerified` and re-check before processing server actions.
- Keep `Secret` on the server only; never expose it to client code.
- Use `VerifyIpAddresses` only when required by your security policy.
- If proxied, ensure `X-Forwarded-For` comes from a trusted source and is properly configured.
- Handle `Expired` and `Error` to prompt users to retry.
- Add rate limiting on endpoints that are protected by CAPTCHA to reduce abuse.

## Warnings
- Blazor WebAssembly is not supported.
- JavaScript is required; the widget will not render without `api.js`.
- Interactive rendering is required; static rendering alone is not enough.

## Limitations
- Blazor WebAssembly is not supported.
- `X-Forwarded-For` is used as-is when proxied; if multiple IPs are present, configure your proxy to send a single, trusted client IP.

## FAQ / Troubleshooting
- **Widget does not render**: Ensure `api.js` is included and the component uses interactive rendering.
- **Always fails verification**: Check that `SiteKey` and `Secret` match your hCaptcha settings and that the server can reach `https://hcaptcha.com/siteverify`.
- **script-error / script blocked**: The hCaptcha JS SDK is blocked (corporate firewall, ad blocker, or CSP).
- **Errors after proxying**: Confirm `X-Forwarded-For` is trustworthy and not being overwritten by untrusted clients.

## Error Codes
| Code                  | Description                                                                                  |
|-----------------------|----------------------------------------------------------------------------------------------|
| **rate-limited**      | User has sent too many requests                                                              |
| **network-error**     | There are network connection issues (e.g., offline). hCaptcha will automatically retry.      |
| **invalid-data**      | Invalid data is not accepted by endpoints                                                    |
| **challenge-error**   | Challenge encountered error on setup. User may need to select the checkbox or call execute.  |
| **challenge-closed**  | User closed the challenge                                                                    |
| **challenge-expired** | Time limit to answer challenge has expired                                                   |
| **missing-captcha**   | No captcha was found. Verify hCaptcha was properly setup and a captcha was rendered.         |
| **invalid-captcha-id**| Captcha does not exist for ID provided. Check that the captcha rendered matches the stored ID. |
| **internal-error**    | hCaptcha client encountered an internal error. User may need to select the checkbox or call execute. |
| **script-error**      | hCaptcha JS SDK could not be loaded. User may be behind a firewall blocking api.js.          |

Source: https://docs.hcaptcha.com/configuration#error-codes

## Links
- GitHub: https://github.com/chataize/captcha
- Chataize organization: https://github.com/chataize
- Website: https://www.chataize.com

