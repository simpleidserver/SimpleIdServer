// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OpenID.Middlewares;
using System.Linq;

namespace SimpleIdServer.OpenID.Extensions
{
    public static class ControllerExtensions
    {
        public static string GetMetadataLanguage(this Controller controller, OAuthHostOptions options)
        {
            if (controller.Request.Headers.ContainsKey(UILocalesMiddleware.AcceptLanguage))
            {
                var strValues = controller.Request.Headers[UILocalesMiddleware.AcceptLanguage];
                if (strValues.Count == 0)
                {
                    return null;
                }

                var requestedLanguages = strValues.First().Split(' ');
                var cultureInfo = TranslationHelper.GetCulture(requestedLanguages, options.SupportedUICultures);
                return cultureInfo == null ? null : cultureInfo.Name;
            }

            return null;
        }
    }
}
