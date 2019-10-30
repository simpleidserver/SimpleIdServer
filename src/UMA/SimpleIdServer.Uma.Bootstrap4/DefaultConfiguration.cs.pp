// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.Uma;
using System.Collections.Generic;

namespace $rootnamespace$
{
    public class DefaultConfiguration
    {
        public static List<OAuthScope> DefaultScopes = new List<OAuthScope>
        {
            UMAConstants.StandardUMAScopes.UmaProtection
        };
    }
}