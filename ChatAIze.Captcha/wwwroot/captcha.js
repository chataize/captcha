export function initCaptcha(id, reference, siteKey, theme, size) {
    let captcha = document.getElementById(id);
    if (!captcha) return;

    if (theme === 'auto') {
        // Match the widget theme to the user's OS preference.
        const prefersDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
        theme = prefersDark ? 'dark' : 'light';
    }

    // Forward hCaptcha events to the Blazor component.
    window.hcaptcha.render(captcha, {
        sitekey: siteKey,
        theme: theme,
        size: size,
        // Success returns a token that must be verified on the server.
        callback: token => reference.invokeMethodAsync('OnCaptchaCompleted', token),
        'open-callback': () => reference.invokeMethodAsync('OnCaptchaOpened'),
        'close-callback': () => reference.invokeMethodAsync('OnCaptchaClosed'),
        'expired-callback': () => reference.invokeMethodAsync('OnCaptchaExpired'),
        'chalexpired-callback': () => reference.invokeMethodAsync('OnCaptchaChallengeExpired'),
        // Error callback passes a code string; surface it to the component.
        'error-callback': code => reference.invokeMethodAsync('OnCaptchaError', code)
    });
}
