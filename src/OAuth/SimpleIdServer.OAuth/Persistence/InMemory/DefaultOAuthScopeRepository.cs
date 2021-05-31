// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Persistence.InMemory
{
    public class DefaultOAuthScopeRepository : InMemoryCommandRepository<OAuthScope>, IOAuthScopeRepository
    {
        public DefaultOAuthScopeRepository(List<OAuthScope> scopes) : base(scopes)
        {
        }

        public Task<List<OAuthScope>> GetAllOAuthScopesExposed(CancellationToken cancellationToken)
        {
            return Task.FromResult(LstData.Where(s => s.IsExposedInConfigurationEdp).ToList());
        }

        public Task<List<OAuthScope>> GetAllOAuthScopes(CancellationToken cancellationToken)
        {
            return Task.FromResult(LstData);
        }

        public Task<List<OAuthScope>> FindOAuthScopesByNames(IEnumerable<string> names, CancellationToken cancellationToken)
        {
            return Task.FromResult(LstData.Where(s => names.Contains(s.Name)).ToList());
        }
    }
}
