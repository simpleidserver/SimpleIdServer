
var reCaptchaV2 = reCaptchaV2 || {};
reCaptchaV2.scriptLoaded = null;

reCaptchaV2.waitScriptLoaded = function (resolve) {
    if (typeof grecaptcha !== 'undefined' && typeof grecaptcha.render !== 'undefined') {
        resolve();
    } else {
        setTimeout(() => reCaptchaV2.waitScriptLoaded(resolve), 100);
    }
};

reCaptchaV2.init = function () {
    const scripts = Array.from(document.getElementsByTagName('script'));
    if (!scripts.some(s => (s.src || '').startsWith('https://www.google.com/recaptcha/api.js'))) {
        const script = document.createElement('script');
        script.src = 'https://www.google.com/recaptcha/api.js?render=explicit';
        script.async = true;
        script.defer = true;
        document.head.appendChild(script);
    }

    if (reCaptchaV2.scriptLoaded === null) {
        reCaptchaV2.scriptLoaded = new Promise(reCaptchaV2.waitScriptLoaded);
    }
    return reCaptchaV2.scriptLoaded;
};

reCaptchaV2.render = function (dotNetObj, selector, siteKey) {
    return new Promise((resolve, reject) => {
        reCaptchaV2.init().then(() => {
            try {
                const widgetId = grecaptcha.render(selector, {
                    'sitekey': siteKey,
                    'callback': (response) => {
                        dotNetObj.invokeMethodAsync('CallbackOnSuccess', response);
                    },
                    'expired-callback': () => {
                        dotNetObj.invokeMethodAsync('CallbackOnExpired');
                    },
                    'error-callback': () => {
                        dotNetObj.invokeMethodAsync('CallbackOnError');
                    }
                });
                resolve(widgetId);
            } catch (error) {
                reject(error);
            }
        }).catch(reject);
    });
};

// Fonction utilitaire pour réinitialiser un widget
reCaptchaV2.reset = function (widgetId) {
    if (typeof grecaptcha !== 'undefined' && typeof grecaptcha.reset !== 'undefined') {
        grecaptcha.reset(widgetId);
    }
};

// Fonction utilitaire pour obtenir la réponse
reCaptchaV2.getResponse = function (widgetId) {
    if (typeof grecaptcha !== 'undefined' && typeof grecaptcha.getResponse !== 'undefined') {
        return grecaptcha.getResponse(widgetId);
    }
    return null;
};