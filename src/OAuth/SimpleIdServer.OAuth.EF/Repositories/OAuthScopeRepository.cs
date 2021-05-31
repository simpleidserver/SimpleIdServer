// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.EF.Repositories
{
    public class OAuthScopeRepository : IOAuthScopeRepository
    {
        private readonly OAuthDBContext _dbContext;

        public OAuthScopeRepository(OAuthDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<OAuthScope>> FindOAuthScopesByNames(IEnumerable<string> names, CancellationToken cancellationToken)
        {
            return GetScopes().Where(s => names.Contains(s.Name)).ToListAsync(cancellationToken);
        }

        public Task<List<OAuthScope>> GetAllOAuthScopes(CancellationToken cancellationToken)
        {
            return GetScopes().ToListAsync(cancellationToken);
        }

        public Task<List<OAuthScope>> GetAllOAuthScopesExposed(CancellationToken cancellationToken)
        {
            return GetScopes().Where(s => s.IsExposedInConfigurationEdp).ToListAsync(cancellationToken);
        }

        private IQueryable<OAuthScope> GetScopes()
        {
            return _dbContext.OAuthScopes.Include(c => c.Claims);
        }

        public Task<bool> Add(OAuthScope data, CancellationToken token)
        {
            _dbContext.OAuthScopes.Add(data);
            return Task.FromResult(true);
        }

        public Task<bool> Delete(OAuthScope data, CancellationToken token)
        {
            _dbContext.OAuthScopes.Remove(data);
            return Task.FromResult(true);
        }

        public Task<bool> Update(OAuthScope data, CancellationToken token)
        {
            _dbContext.OAuthScopes.Update(data);
            return Task.FromResult(true);
        }

        public Task<int> SaveChanges(CancellationToken token)
        {
            return _dbContext.SaveChangesAsync(token);
        }

        public void Dispose()
        {
        }
    }
}
