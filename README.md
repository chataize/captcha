# CAPTCHA
C# .NET 8.0 Blazor server component for [hCaptcha](https://www.hcaptcha.com).

> [!WARNING]
> This library is designed to be used in a Blazor server app with interactive rendering enabled. It does **NOT** support Blazor WebAssembly.

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
### Service (Program.cs)
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
> If users are connecting to your app via a reverse proxy like [Cloudflare](https://www.cloudflare.com), `IsConnectionProxied` should be set to `true`.
> In such a case, the `X-Forwarded-For` header will be used to determine the client's IP address.
### Middleware (Program.cs)
```cs
app.UseCaptcha();
```
### Javascript (App.razor)
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

## Usage
```razor
<Captcha @bind-IsVerified="_isVerified" />
<h1>Verified: @_isVerified</h1>

@code {
    private bool _isVerified;
}
```
> [!TIP]
> Make sure that the component containing the CAPTCHA has **interactive rendering** enabled.
> ```razor
> @rendermode InteractiveServer
> ```
### Properties
- **SiteKey**: Overrides service settings
- **Secret**: Overrides service settings
```razor
<Captcha @bind-IsVerified="_isVerified" SiteKey="" Secret="" />
```
- **Theme**: Auto - Light - Dark
- **Size**: Normal - Compact
```razor
<Captcha @bind-IsVerified="_isVerified" Theme="CaptchaTheme.Auto" Size="CaptchaSize.Normal" />
```
### Events
- **Opened**: Called when the user display of a challenge starts.
- **Closed**: Called when the user dismisses a challenge.
- **Succeeded**: Called when the user submits a successful response.
- **Expired**: Called when the passcode response expires and the user must re-verify.
- **ChallengeExpired**: Called when the user display of a challenge times out with no answer.
- **Error**: Called when hCaptcha encounters an error and cannot continue.
```razor
<Captcha @bind-IsVerified="_isVerified" SiteKey="" Secret="" Theme="CaptchaTheme.Auto" Size="CaptchaSize.Normal" Opened="OnOpened" Closed="OnClosed" Succeeded="OnSucceeded" Expired="OnExpired" ChallengeExpired="OnChallengeExpired" Error="OnError" />

@code {
    private bool _isVerified;

    private void OnOpened() { }

    private void OnClosed() { }

    private void OnSucceeded() { }

    private void OnExpired() { }

    private void OnChallengeExpired() { }

    private void OnError(string code) { }
```
### Errors
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
