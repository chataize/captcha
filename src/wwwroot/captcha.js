export function initCaptcha(id, reference, siteKey, theme, size) {
    let captcha = document.getElementById(id);
    if (!captcha) return;

    if (theme === 'auto') {
        const prefersDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
        theme = prefersDark ? 'dark' : 'light';
    }

    window.hcaptcha.render(captcha, {
        sitekey: siteKey,
        theme: theme,
        size: size,
        callback: token => reference.invokeMethodAsync('OnCaptchaCompleted', token),
        'open-callback': () => reference.invokeMethodAsync('OnCaptchaOpened'),
        'close-callback': () => reference.invokeMethodAsync('OnCaptchaClosed'),
        'expired-callback': () => reference.invokeMethodAsync('OnCaptchaExpired'),
        'chalexpired-callback': () => reference.invokeMethodAsync('OnCaptchaChallengeExpired'),
        'error-callback': code => reference.invokeMethodAsync('OnCaptchaError', code)
    });
}
