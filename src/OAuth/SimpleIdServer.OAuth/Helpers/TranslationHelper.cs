// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using SimpleIdServer.Common;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using System.Collections.Generic;
using System.Globalization;

namespace SimpleIdServer.OAuth.Helpers
{
    public class TranslationHelper : ITranslationHelper
    {
        private readonly SimpleIdServerCommonOptions _commonOptions;

        public TranslationHelper(IOptions<SimpleIdServerCommonOptions> commonOptions)
        {
            _commonOptions = commonOptions.Value;
        }

        public string Translate(ICollection<OAuthTranslation> translations, string defaultValue = null)
        {
            var defaultLanguage = CultureInfo.DefaultThreadCurrentUICulture != null ? CultureInfo.DefaultThreadCurrentUICulture.Name : _commonOptions.DefaultCulture;
            return translations.GetTranslation(defaultLanguage, defaultValue);
        }
    }
}
