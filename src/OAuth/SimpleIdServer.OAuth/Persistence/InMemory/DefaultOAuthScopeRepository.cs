// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence.Parameters;
using SimpleIdServer.OAuth.Persistence.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Persistence.InMemory
{
    public class DefaultOAuthScopeRepository : InMemoryCommandRepository<OAuthScope>, IOAuthScopeRepository
    {
        private static Dictionary<string, string> MAPPING_SCOPE_TO_PROPERTYNAME = new Dictionary<string, string>
        {
            { "name", "Name" },
            { "create_datetime", "CreateDateTime" },
            { "update_datetime", "UpdateDateTime" }
        };

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

        public Task<SearchResult<OAuthScope>> Find(SearchScopeParameter parameter, CancellationToken cancellationToken)
        {
            var result = LstData.AsQueryable();
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
            return Task.FromResult(LstData.FirstOrDefault(s => s.Name == name));
        }
    }
}
