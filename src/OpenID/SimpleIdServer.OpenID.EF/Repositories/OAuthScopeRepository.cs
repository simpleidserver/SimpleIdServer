// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OAuth.Persistence.Parameters;
using SimpleIdServer.OAuth.Persistence.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.EF.Repositories
{
    public class OAuthScopeRepository : IOAuthScopeRepository
    {
        private static Dictionary<string, string> MAPPING_SCOPE_TO_PROPERTYNAME = new Dictionary<string, string>
        {
            { "name", "Name" },
            { "create_datetime", "CreateDateTime" },
            { "update_datetime", "UpdateDateTime" }
        };
        private readonly OpenIdDBContext _dbContext;

        public OAuthScopeRepository(OpenIdDBContext dbContext)
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

        public async Task<SearchResult<OAuthScope>> Find(SearchScopeParameter parameter, CancellationToken cancellationToken)
        {
            var result = _dbContext.OAuthScopes.AsQueryable();
            if (MAPPING_SCOPE_TO_PROPERTYNAME.ContainsKey(parameter.OrderBy))
            {
                result = result.InvokeOrderBy(MAPPING_SCOPE_TO_PROPERTYNAME[parameter.OrderBy], parameter.Order);
            }

            int totalLength = result.Count();
            var content = await result.Skip(parameter.StartIndex).Take(parameter.Count).ToListAsync(cancellationToken);
            return new SearchResult<OAuthScope>
            {
                StartIndex = parameter.StartIndex,
                Count = parameter.Count,
                TotalLength = totalLength,
                Content = content
            };
        }

        public Task<OAuthScope> GetOAuthScope(string name, CancellationToken cancellationToken)
        {
            return _dbContext.OAuthScopes.Include(s => s.Claims).FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
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
