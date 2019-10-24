// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Uma.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Persistence.InMemory
{
    public class DefaultUMAPendingRequestQueryRepository : IUMAPendingRequestQueryRepository
    {
        private List<UMAPendingRequest> _umaPendingRequests;

        public DefaultUMAPendingRequestQueryRepository(List<UMAPendingRequest> umaPendingRequests)
        {
            _umaPendingRequests = umaPendingRequests;
        }

        public Task<SearchResult<UMAPendingRequest>> FindByOwner(string owner, SearchRequestParameter parameter)
        {
            var filteredUMAPendingRequests = _umaPendingRequests.AsQueryable();
            filteredUMAPendingRequests = filteredUMAPendingRequests.Where(f => f.Owner == owner);
            return InternalFind(filteredUMAPendingRequests, parameter);
        }

        public Task<SearchResult<UMAPendingRequest>> FindByRequester(string requester, SearchRequestParameter parameter)
        {
            var filteredUMAPendingRequests = _umaPendingRequests.AsQueryable();
            filteredUMAPendingRequests = filteredUMAPendingRequests.Where(f => f.Requester == requester);
            return InternalFind(filteredUMAPendingRequests, parameter);
        }

        public Task<IEnumerable<UMAPendingRequest>> FindByTicketIdentifier(string ticketIdentifier)
        {
            return Task.FromResult(_umaPendingRequests.Where(r => r.TicketId == ticketIdentifier));
        }

        public Task<UMAPendingRequest> FindByTicketIdentifierAndOwner(string ticketIdentifier, string owner)
        {
            return Task.FromResult(_umaPendingRequests.FirstOrDefault(r => r.TicketId == ticketIdentifier && r.Owner == owner));
        }

        private Task<SearchResult<UMAPendingRequest>> InternalFind(IQueryable<UMAPendingRequest> result, SearchRequestParameter parameter)
        {
            var totalResults = result.Count();
            if (parameter.SortKey == "CreateDateTime")
            {
                switch (parameter.SortOrder)
                {
                    case SearchRequestOrders.ASC:
                        result = result.OrderBy(p => p.CreateDateTime);
                        break;
                    case SearchRequestOrders.DESC:
                        result = result.OrderByDescending(p => p.CreateDateTime);
                        break;
                }
            }

            result = result.Skip(parameter.StartIndex).Take(parameter.Count);
            return Task.FromResult(new SearchResult<UMAPendingRequest>
            {
                Records = result.ToList(),
                TotalResults = totalResults
            });
        }
    }
}
