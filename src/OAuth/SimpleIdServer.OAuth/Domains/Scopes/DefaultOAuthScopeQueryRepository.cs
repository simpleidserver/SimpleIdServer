// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Domains.Scopes
{
    public class DefaultOAuthScopeQueryRepository : IOAuthScopeQueryRepository
    {
        private readonly List<OAuthScope> _scopes;

        public DefaultOAuthScopeQueryRepository(List<OAuthScope> scopes)
        {
            _scopes = scopes;
        }

        public Task<List<OAuthScope>> GetAllOAuthScopesExposed()
        {
            return Task.FromResult(_scopes.Where(s => s.IsExposedInConfigurationEdp).ToList());
        }

        public Task<List<OAuthScope>> GetAllOAuthScopes()
        {
            return Task.FromResult(_scopes);
        }

        public Task<List<OAuthScope>> FindOAuthScopesByNames(IEnumerable<string> names)
        {
            return Task.FromResult(_scopes.Where(s => names.Contains(s.Name)).ToList());
        }
    }
}
