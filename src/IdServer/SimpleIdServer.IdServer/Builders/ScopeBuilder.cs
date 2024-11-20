// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Builders
{
    public class ScopeBuilder
    {
        private readonly Scope _scope;

        private ScopeBuilder(Scope scope)
        {
            _scope = scope;
        }

        public static ScopeBuilder Create(string name, bool isExposed = false, Realm realm = null)
        {
            return new ScopeBuilder(new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                CreateDateTime = DateTime.Now,
                UpdateDateTime  = DateTime.Now,
                IsExposedInConfigurationEdp = isExposed,
                Realms = new List<Domains.Realm>
                {
                    realm ?? Constants.StandardRealms.Master
                }
            });
        }

        public static ScopeBuilder CreateApiScope(string name, bool isExposed = false)
        {
            return new ScopeBuilder(new Scope 
            { 
                Id = Guid.NewGuid().ToString(),
                Name = name, 
                IsExposedInConfigurationEdp = isExposed,
                Realms = new List<Domains.Realm>
                {
                    Constants.StandardRealms.Master
                }
            });
        }

        public static ScopeBuilder CreateRoleScope(Client client, string name, string description, Domains.Realm realm = null)
        {
            return new ScopeBuilder(new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Name = $"{client.ClientId}/{name}",
                Type = ScopeTypes.ROLE,
                Protocol = ScopeProtocols.OAUTH,
                Description = description,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                Realms = new List<Domains.Realm>
                {
                    realm ?? Constants.StandardRealms.Master
                },
                Clients = new List<Client>
                { 
                    client
                }
            });
        }

        public ScopeBuilder SetDescription(string description)
        {
            _scope.Description = description;
            return this;
        }

        public ScopeBuilder SetType(ScopeTypes type)
        {
            _scope.Type = type;
            return this;
        }

        public ScopeBuilder SetProtocol(ScopeProtocols protocol)
        {
            _scope.Protocol = protocol;
            return this;
        }

        public ScopeBuilder SetComponent(string component)
        {
            _scope.Component = component;
            return this;
        }

        public ScopeBuilder SetComponentAction(ComponentActions componentAction)
        {
            _scope.Action = componentAction;
            return this;
        }

        public Scope Build() => _scope;
    }
}
