// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.OpenBankingApi.Host.Acceptance.Tests
{
    public class DefaultConfiguration
    {
        public static List<OAuthScope> Scopes => new List<OAuthScope>
        {
            new OAuthScope
            {
                Name = "accounts",
                IsExposedInConfigurationEdp = true
            }
        };
    }
}