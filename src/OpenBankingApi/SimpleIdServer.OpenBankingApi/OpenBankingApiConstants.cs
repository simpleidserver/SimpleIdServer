// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.OpenID.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.OpenBankingApi
{
    public static class OpenBankingApiConstants
    {
        public static class OpenBankingApiScopes
        {
            public static OpenIdScope Accounts = new OpenIdScope("accounts")
            {
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                Claims = new List<OpenIdScopeClaim>
                {
                    new OpenIdScopeClaim("acr", true),
                    new OpenIdScopeClaim("openbanking_intent_id", true)
                }
            };
        }
    }
}
