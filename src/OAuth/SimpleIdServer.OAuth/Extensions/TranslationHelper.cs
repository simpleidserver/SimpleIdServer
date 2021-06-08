// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SimpleIdServer.OAuth.Extensions
{
    public static class TranslationHelper
    {
        public const string AcceptLanguage = "Accept-Language";

        public static CultureInfo GetCulture(IEnumerable<string> requestedCultures, IEnumerable<UICultureOption> uiCultures)
        {
            var culture = requestedCultures.FirstOrDefault(r => uiCultures.Any(sc => sc.Name == r));
            if (culture == null)
            {
                return null;
            }

            return new CultureInfo(culture);
        }

        public static bool TrySwitchCulture(IEnumerable<string> requestedCultures, IEnumerable<UICultureOption> uiCultures)
        {
            var cultureInfo = GetCulture(requestedCultures, uiCultures);
            if (cultureInfo != null)
            {
                SwitchCulture(cultureInfo);
                return true;
            }

            return false;
        }

        public static void SwitchCulture(string language)
        {
            SwitchCulture(new CultureInfo(language));
        }

        public static void SwitchCulture(CultureInfo cultureInfo)
        {
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }
    }
}
