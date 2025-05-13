// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.Builders;

public class RealmRoleBuilder
{
    private static List<string> _allComponents = new List<string>
    {
        "realms",
        "clients",
        "scopes",
        "users",
        "groups",
        "acrs",
        "authentication",
        "manualidprovisioning",
        "automaticidprovisioning",
        "certificateauthorities",
        "auditing",
        "trustanchors",
        "recurringjobs",
        "migrations"
    };

    public static List<Scope> BuildAdministrativeRole(Realm realm)
    {
        var realmScopes = BuildScopes(realm);
        return realmScopes;
    }

    public static List<Scope> BuildAdministrativeRoleRo(Realm realm)
    {
        var realmScopes = BuildScopes(realm, true);
        return realmScopes;
    }

    private static List<Scope> BuildScopes(Realm realm, bool isReadOnly = false)
        => _allComponents.SelectMany(c => BuildComponentScopes(c, realm, isReadOnly)).ToList();

    private static List<Scope> BuildComponentScopes(string componentName, Realm realm, bool isReadOnly = false)
    {
        var roles = new List<Scope>
        {
            new Domains.Scope
            {
                Id = $"{realm.Name}/{componentName}/view",
                Name = $"{realm.Name}/{componentName}/view",
                Component = componentName,
                Action = ComponentActions.View,
                Type = ScopeTypes.ROLE,
                Realms = new List<Domains.Realm>
                {
                    realm
                },
                Protocol = ScopeProtocols.OAUTH,
                Description = $"View {realm.Name} {componentName}",
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
            }
        };
        if (!isReadOnly)
        {
            roles.Add(new Domains.Scope
            {
                Id = $"{realm.Name}/{componentName}/manage",
                Name = $"{realm.Name}/{componentName}/manage",
                Component = componentName,
                Action = ComponentActions.Manage,
                Type = ScopeTypes.ROLE,
                Realms = new List<Domains.Realm>
            {
                realm
            },
                Protocol = ScopeProtocols.OAUTH,
                Description = $"Manage {realm.Name} {componentName}",
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
            });
        }

        return roles;
    }
}