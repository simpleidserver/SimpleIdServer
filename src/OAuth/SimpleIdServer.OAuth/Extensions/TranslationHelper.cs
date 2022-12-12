// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Globalization;

namespace SimpleIdServer.OAuth.Extensions
{
    public static class TranslationHelper
    {
        public const string AcceptLanguage = "Accept-Language";

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
