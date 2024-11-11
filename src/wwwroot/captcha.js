export function initCaptcha(id, reference, siteKey, theme, size) {
    let captcha = document.getElementById(id);
    if (!captcha) return;

    window.hcaptcha.render(captcha, {
        sitekey: siteKey,
        theme: theme,
        size: size,
        callback: function (token) {
            reference.invokeMethodAsync('OnCaptchaCompleted', token);
        }
    });
}
