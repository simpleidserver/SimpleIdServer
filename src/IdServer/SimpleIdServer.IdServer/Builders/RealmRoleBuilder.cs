// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Builders;

public class RealmRoleBuilder
{
    public static RealmRole BuildAdministrativeRole(Realm realm)
    {
        var realmClientsManageRole = new Domains.Scope
        {
            Id = Guid.NewGuid().ToString(),
            Name = $"{realm.Name}/clients/manage",
            Component = "clients",
            Action = ComponentActions.Manage,
            Type = ScopeTypes.ROLE,
            Realms = new List<Domains.Realm>
            {
                realm
            },
            Protocol = ScopeProtocols.OAUTH,
            Description = "Manage master clients",
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow,
        };
        var realmClientsViewRole = new Domains.Scope
        {
            Id = Guid.NewGuid().ToString(),
            Name = $"{realm.Name}/clients/view",
            Component = "clients",
            Action = ComponentActions.View,
            Type = ScopeTypes.ROLE,
            Realms = new List<Domains.Realm>
            {
                realm
            },
            Protocol = ScopeProtocols.OAUTH,
            Description = "View master clients",
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow,
        };
        return new RealmRole
        {
            Id = Guid.NewGuid().ToString(),
            Name = "administrator",
            RealmName = realm.Name,
            Scopes = new List<RealmRoleScope>
            {
                new RealmRoleScope
                {
                    Scope = realmClientsManageRole
                },
                new RealmRoleScope
                {
                    Scope = realmClientsViewRole
                }
            },
            UpdateDateTime = DateTime.UtcNow,
            CreateDateTime = DateTime.UtcNow
        };
    }
}