using ChatAIze.Captcha;
using ChatAIze.Captcha.Preview.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCaptcha(o =>
{
    o.SiteKey = builder.Configuration["Captcha:SiteKey"]!;
    o.Secret = builder.Configuration["Captcha:Secret"]!;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseStaticFiles();
app.UseCaptcha();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
