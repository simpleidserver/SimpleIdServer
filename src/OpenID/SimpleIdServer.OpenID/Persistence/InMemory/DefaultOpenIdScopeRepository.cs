// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OAuth.Persistence.Parameters;
using SimpleIdServer.OAuth.Persistence.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Persistence.InMemory
{
    public class DefaultOpenIdScopeRepository : IOAuthScopeRepository
    {
        private static Dictionary<string, string> MAPPING_SCOPE_TO_PROPERTYNAME = new Dictionary<string, string>
        {
            { "name", "Name" },
            { "create_datetime", "CreateDateTime" },
            { "update_datetime", "UpdateDateTime" }
        };
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

        public Task<SearchResult<OAuthScope>> Find(SearchScopeParameter parameter, CancellationToken cancellationToken)
        {
            var result = _scopes.AsQueryable();
            if (MAPPING_SCOPE_TO_PROPERTYNAME.ContainsKey(parameter.OrderBy))
            {
                result = result.InvokeOrderBy(MAPPING_SCOPE_TO_PROPERTYNAME[parameter.OrderBy], parameter.Order);
            }

            int totalLength = result.Count();
            result = result.Skip(parameter.StartIndex).Take(parameter.Count);
            return Task.FromResult(new SearchResult<OAuthScope>
            {
                StartIndex = parameter.StartIndex,
                Count = parameter.Count,
                TotalLength = totalLength,
                Content = result.ToList()
            });
        }

        public Task<OAuthScope> GetOAuthScope(string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(_scopes.FirstOrDefault(s => s.Name == name));
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
