// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Persistence.InMemory
{
    public class DefaultOpenIdScopeCommandRepository : IOAuthScopeCommandRepository
    {
        private readonly List<OpenIdScope> _scopes;

        public DefaultOpenIdScopeCommandRepository(List<OpenIdScope> scopes)
        {
            _scopes = scopes;
        }

        public bool Add(OAuthScope data)
        {
            _scopes.Add((OpenIdScope)data.Clone());
            return true;
        }

        public bool Update(OAuthScope data)
        {
            _scopes.Remove(_scopes.First(s => s.Name == data.Name));
            _scopes.Add((OpenIdScope)data.Clone());
            return true;
        }

        public bool Delete(OAuthScope data)
        {
            _scopes.Remove(_scopes.First(s => s.Name == data.Name));
            return true;
        }

        public Task<int> SaveChanges()
        {
            return Task.FromResult(1);
        }

        public void Dispose()
        {
        }
    }
}
