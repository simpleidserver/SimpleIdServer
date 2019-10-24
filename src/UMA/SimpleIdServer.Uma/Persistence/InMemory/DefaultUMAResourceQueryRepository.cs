// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Uma.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Persistence.InMemory
{
    public class DefaultUMAResourceQueryRepository : IUMAResourceQueryRepository
    {
        private List<UMAResource> _umaResources;

        public DefaultUMAResourceQueryRepository(List<UMAResource> umaResources)
        {
            _umaResources = umaResources;
        }

        public Task<UMAResource> FindByIdentifier(string id)
        {
            return Task.FromResult(_umaResources.FirstOrDefault(r => r.Id == id));
        }

        public Task<IEnumerable<UMAResource>> FindByIdentifiers(IEnumerable<string> ids)
        {
            return Task.FromResult(_umaResources.Where(r => ids.Contains(r.Id)));
        }

        public Task<IEnumerable<UMAResource>> GetAll()
        {
            return Task.FromResult((IEnumerable<UMAResource>)_umaResources);
        }

        public Task<SearchResult<UMAResource>> Find(SearchUMAResourceParameter searchRequestParameter)
        {
            var filteredUmaResources = _umaResources.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchRequestParameter.Subject))
            {
                filteredUmaResources = filteredUmaResources.Where(f => f.Subject == searchRequestParameter.Subject);
            }

            int totalResults = _umaResources.Count();
            if (searchRequestParameter.SortKey == "CreateDateTime")
            {
                switch(searchRequestParameter.SortOrder)
                {
                    case SearchRequestOrders.ASC:
                        filteredUmaResources = filteredUmaResources.OrderBy(p => p.CreateDateTime);
                        break;
                    case SearchRequestOrders.DESC:
                        filteredUmaResources = filteredUmaResources.OrderByDescending(p => p.CreateDateTime);
                        break;
                }
            }

            filteredUmaResources = filteredUmaResources.Skip(searchRequestParameter.StartIndex).Take(searchRequestParameter.Count);
            return Task.FromResult(new SearchResult<UMAResource>
            {
                Records = filteredUmaResources.ToList(),
                TotalResults = totalResults
            });
        }
    }
}
