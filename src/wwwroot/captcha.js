export function initCaptcha(id, reference, siteKey, theme, size) {
    let captcha = document.getElementById(id);
    if (!captcha) return;

    window.hcaptcha.render(captcha, {
        sitekey: siteKey,
        theme: theme,
        size: size,
        callback: function (token) {
            reference.invokeMethodAsync('OnCaptchaCompleted', token);
        },
        'open-callback': function () {
            reference.invokeMethodAsync('OnCaptchaOpened');
        },
        'close-callback': function () {
            reference.invokeMethodAsync('OnCaptchaClosed');
        },
        'expired-callback': function () {
            reference.invokeMethodAsync('OnCaptchaExpired');
        },
        'chalexpired-callback': function () {
            reference.invokeMethodAsync('OnCaptchaChallengeExpired');
        },
        'error-callback': function (code) {
            reference.invokeMethodAsync('OnCaptchaError', code);
        }
    });
}
