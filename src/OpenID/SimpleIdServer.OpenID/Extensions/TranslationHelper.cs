// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SimpleIdServer.OpenID.Extensions
{
    public static class TranslationHelper
    {
        public static CultureInfo GetCulture(IEnumerable<string> requestedCultures, string defaultCulture = null)
        {
            var rootDir = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            var supportedCultures = from c in CultureInfo.GetCultures(CultureTypes.AllCultures)
                                    join d in rootDir.EnumerateDirectories() on c.IetfLanguageTag equals d.Name
                                    select c;
            var culture = requestedCultures.FirstOrDefault(r => supportedCultures.Any(sc => sc.Name == r));
            if (culture == null && !string.IsNullOrWhiteSpace(defaultCulture))
            {
                culture = defaultCulture;
            }

            return new CultureInfo(culture);
        }

        public static string SwitchCulture(IEnumerable<string> requestedCultures, string defaultCulture)
        {
            var cultureInfo = GetCulture(requestedCultures, defaultCulture);
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            return cultureInfo.Name;
        }
    }
}
