export function initCaptcha(id, reference, siteKey, theme) {
    let captcha = document.getElementById(id);
    if (!captcha) return;

    window.hcaptcha.render(captcha, {
        sitekey: siteKey,
        theme: theme,
        callback: function (token) {
            reference.invokeMethodAsync('OnCaptchaCompleted', token);
        }
    });
}
