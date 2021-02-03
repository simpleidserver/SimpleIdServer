// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Persistence.InMemory
{
    public class DefaultOpenIdScopeQueryRepository : IOAuthScopeQueryRepository
    {
        private readonly List<OpenIdScope> _scopes;

        public DefaultOpenIdScopeQueryRepository(List<OpenIdScope> scopes)
        {
            _scopes = scopes;
        }

        public Task<List<OAuthScope>> FindOAuthScopesByNames(IEnumerable<string> names, CancellationToken token)
        {
            return Task.FromResult(_scopes.Where(s => names.Contains(s.Name)).Cast<OAuthScope>().ToList());
        }

        public Task<List<OAuthScope>> GetAllOAuthScopes()
        {
            return Task.FromResult(_scopes.Cast<OAuthScope>().ToList());
        }

        public Task<List<OAuthScope>> GetAllOAuthScopesExposed()
        {
            return Task.FromResult(_scopes.Where(s => s.IsExposedInConfigurationEdp).Cast<OAuthScope>().ToList());
        }
    }
}
