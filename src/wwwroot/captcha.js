export function initCaptcha(id, reference, siteKey) {
    let captcha = document.getElementById(id);
    if (!captcha) return;

    window.hcaptcha.render('captcha', {
        sitekey: siteKey,
        callback: function (token) {
            reference.invokeMethodAsync('OnCaptchaCompleted', token);
        }
    });
}
