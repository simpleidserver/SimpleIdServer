// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Persistence.InMemory;
using SimpleIdServer.Uma.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Persistence.InMemory
{
    public class DefaultUMAResourceRepository : InMemoryCommandRepository<UMAResource>, IUMAResourceRepository
    {
        public DefaultUMAResourceRepository(List<UMAResource> umaResources) : base(umaResources)
        {
        }

        public Task<UMAResource> FindByIdentifier(string id, CancellationToken token)
        {
            return Task.FromResult(LstData.FirstOrDefault(r => r.Id == id));
        }

        public Task<IEnumerable<UMAResource>> FindByIdentifiers(IEnumerable<string> ids, CancellationToken token)
        {
            return Task.FromResult(LstData.Where(r => ids.Contains(r.Id)));
        }

        public Task<IEnumerable<UMAResource>> GetAll(CancellationToken token)
        {
            return Task.FromResult((IEnumerable<UMAResource>)LstData);
        }

        public Task<SearchResult<UMAResource>> Find(SearchUMAResourceParameter searchRequestParameter, CancellationToken token)
        {
            var filteredUmaResources = LstData.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchRequestParameter.Subject))
            {
                filteredUmaResources = filteredUmaResources.Where(f => f.Subject == searchRequestParameter.Subject);
            }

            int totalResults = LstData.Count();
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
