// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Options;
using System.Collections.Generic;
using System.Globalization;

namespace SimpleIdServer.OAuth.Helpers
{
    public class TranslationHelper : ITranslationHelper
    {
        private readonly OAuthHostOptions _oauthHostOptions;

        public TranslationHelper(IOptions<OAuthHostOptions> options)
        {
            _oauthHostOptions = options.Value;
        }

        public string Translate(ICollection<OAuthTranslation> translations, string defaultValue = null)
        {
            var defaultLanguage = CultureInfo.DefaultThreadCurrentUICulture != null ? CultureInfo.DefaultThreadCurrentUICulture.Name : _oauthHostOptions.DefaultCulture;
            return translations.GetTranslation(defaultLanguage, defaultValue);
        }
    }
}
