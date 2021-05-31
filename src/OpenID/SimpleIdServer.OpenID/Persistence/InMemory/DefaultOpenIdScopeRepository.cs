// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Persistence.InMemory
{
    public class DefaultOpenIdScopeRepository : IOAuthScopeRepository
    {
        private readonly List<OAuthScope> _scopes;

        public DefaultOpenIdScopeRepository(List<OAuthScope> scopes)
        {
            _scopes = scopes;
        }

        public Task<List<OAuthScope>> FindOAuthScopesByNames(IEnumerable<string> names, CancellationToken token)
        {
            return Task.FromResult(_scopes.Where(s => names.Contains(s.Name)).Cast<OAuthScope>().ToList());
        }

        public Task<List<OAuthScope>> GetAllOAuthScopes(CancellationToken token)
        {
            return Task.FromResult(_scopes.Cast<OAuthScope>().ToList());
        }

        public Task<List<OAuthScope>> GetAllOAuthScopesExposed(CancellationToken token)
        {
            return Task.FromResult(_scopes.Where(s => s.IsExposedInConfigurationEdp).Cast<OAuthScope>().ToList());
        }

        public Task<bool> Add(OAuthScope data, CancellationToken cancellationToken)
        {
            _scopes.Add((OAuthScope)data.Clone());
            return Task.FromResult(true);
        }

        public Task<bool> Update(OAuthScope data, CancellationToken token)
        {
            _scopes.Remove(_scopes.First(s => s.Name == data.Name));
            _scopes.Add((OAuthScope)data.Clone());
            return Task.FromResult(true);
        }

        public Task<bool> Delete(OAuthScope data, CancellationToken cancellationToken)
        {
            _scopes.Remove(_scopes.First(s => s.Name == data.Name));
            return Task.FromResult(true);
        }

        public Task<int> SaveChanges(CancellationToken token)
        {
            return Task.FromResult(1);
        }

        public void Dispose()
        {
        }
    }
}
