// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using static SimpleIdServer.IdServer.Constants;

namespace SimpleIdServer.Configuration
{
    public static class Constants
    {
        public static class RouteNames
        {
            public const string ConfigurationDefs = "confdefs";
        }


        public static Scope ConfigurationsScope = new Scope
        {
            Id = Guid.NewGuid().ToString(),
            Name = "configurations",
            Realms = new List<Realm>
            {
                StandardRealms.Master
            },
            Type = ScopeTypes.APIRESOURCE,
            Protocol = ScopeProtocols.OAUTH,
            IsExposedInConfigurationEdp = true,
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow
        };
    }
}
