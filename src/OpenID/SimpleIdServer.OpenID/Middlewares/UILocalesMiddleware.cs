// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OpenID.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Middlewares
{
    public class UILocalesMiddleware
    {
        private readonly RequestDelegate _next;

        public UILocalesMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var opts = (IOptions<OAuthHostOptions>)context.RequestServices.GetService(typeof(IOptions<OAuthHostOptions>));
            if (context.Request.Query.ContainsKey(AuthorizationRequestParameters.UILocales) && context.Request.Query[AuthorizationRequestParameters.UILocales].Count == 1)
            {
                var uiLocales = context.Request.Query[AuthorizationRequestParameters.UILocales][0];
                if (!string.IsNullOrWhiteSpace(uiLocales))
                {
                    var splittedUILocales = uiLocales.Split(' ');
                    if (TranslationHelper.TrySwitchCulture(splittedUILocales, opts.Value.SupportedUICultures))
                    {
                        await _next(context);
                        return;
                    }
                }
            }

            if (context.Request.Cookies.ContainsKey(CookieRequestCultureProvider.DefaultCookieName))
            {
                var str = context.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
                var value = CookieRequestCultureProvider.ParseCookieValue(str);
                if (TranslationHelper.TrySwitchCulture(value.UICultures.Select(_ => _.ToString()), opts.Value.SupportedUICultures))
                {
                    await _next(context);
                    return;
                }
            }

            TranslationHelper.SwitchCulture(opts.Value.DefaultCulture);
            await _next(context);
        }
    }
}
