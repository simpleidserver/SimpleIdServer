// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Config;

public static class DefaultGroups
{
    public static List<string> AllFullPath => new List<string>
    {
        AdministratorGroup.FullPath,
        AdministratorReadonlyGroup.FullPath,
    };

    public static List<Group> All => new List<Group>
    {
        AdministratorGroup,
        AdministratorReadonlyGroup
    };

    public static Group AdministratorGroup
    {
        get
        {
            var roles = new List<Scope>();
            roles.AddRange(DefaultScopes.AdministrativeScopes);
            return new Group
            {
                Id = "9795f2aa-3a86-4e21-a098-d0443e0391d4",
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                FullPath = "administrator",
                Realms = new List<GroupRealm>
                {
                    new GroupRealm
                    {
                        RealmsName = DefaultRealms.Master.Name
                    }
                },
                Name = "administrator",
                Description = "Administration role",
                Roles = roles
            };
        }
    }

    public static Group AdministratorReadonlyGroup = new Group
    {
        Id = "7a3014a3-5985-4986-bfcc-8e574fb6da27",
        CreateDateTime = DateTime.UtcNow,
        FullPath = "administrator-ro",
        Realms = new List<GroupRealm>
        {
            new GroupRealm
            {
                RealmsName = DefaultRealms.Master.Name
            }
        },
        Name = "administrator-ro",
        Description = "Administration role readonly",
        Roles = DefaultScopes.AdministrativeRoScopes
    };
}
