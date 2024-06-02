// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;

namespace SimpleIdServer.IdServer.Builders
{
    public class GroupBuilder
    {
        private readonly Group _group;

        public GroupBuilder(Group group)
        {
            _group = group;
        }

        public static GroupBuilder Create(string name, string description)
        {
            var record = new Group { Id = Guid.NewGuid().ToString(), Name = name, Description = description, FullPath = name };
            return new GroupBuilder(record);
        }

        public GroupBuilder AddRole(Scope scope)
        {
            _group.Roles.Add(scope);
            return this;
        }

        public GroupBuilder AddRealm(string realm)
        {
            _group.Realms.Add(new GroupRealm
            {
                RealmsName = realm
            });
            return this;
        }

        public Group Build() => _group;
    }
}
