// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Options;
using System.Linq;

namespace Microsoft.AspNetCore.Mvc
{
    public static class ControllerExtensions
    {
        public static string GetLanguage(this Controller controller, OAuthHostOptions options)
        {
            if (controller.Request.Headers.ContainsKey(TranslationHelper.AcceptLanguage))
            {
                var strValues = controller.Request.Headers[TranslationHelper.AcceptLanguage];
                if (strValues.Count == 0) return null;
                var requestedLanguages = strValues.First().Split(' ');
                var cultureInfo = TranslationHelper.GetCulture(requestedLanguages, options.SupportedUICultures);
                return cultureInfo == null ? null : cultureInfo.Name;
            }

            return null;
        }
    }
}
