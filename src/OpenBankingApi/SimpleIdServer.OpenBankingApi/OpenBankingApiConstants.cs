// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.OAuth.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.OpenBankingApi
{
    public static class OpenBankingApiConstants
    {
        public static class NotifiableConsentTypes
        {
            public const string AccountAccessConsent = "AccountAccessConsent";
        }

        public static class OpenBankingApiScopes
        {
            public static OAuthScope Accounts = new OAuthScope("accounts")
            {
                IsExposedInConfigurationEdp = true,
                IsStandardScope = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                Claims = new List<OAuthScopeClaim>
                {
                    new OAuthScopeClaim("acr", true),
                    new OAuthScopeClaim("openbanking_intent_id", true)
                }
            };
        }
    }
}
