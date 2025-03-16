﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Federation;

public static class IdServerFederationConstants
{
    public static class StandardScopes
    {
        public static Scope FederationEntities = new Scope
        {
            Id = Guid.NewGuid().ToString(),
            Type = ScopeTypes.APIRESOURCE,
            Name = "federation_entities",
            Realms = new List<Realm>
            {
                Config.DefaultRealms.Master
            },
            Protocol = ScopeProtocols.OAUTH,
            IsExposedInConfigurationEdp = true,
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow
        };
    }
}
