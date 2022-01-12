// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Persistence.InMemory;
using SimpleIdServer.Uma.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Persistence.InMemory
{
    public class DefaultUMAPendingRequestRepository : InMemoryCommandRepository<UMAPendingRequest>, IUMAPendingRequestRepository
    {
        public DefaultUMAPendingRequestRepository(List<UMAPendingRequest> umaPendingRequests) : base(umaPendingRequests)
        {
        }

        public Task<SearchResult<UMAPendingRequest>> Find(SearchRequestParameter parameter, CancellationToken cancellationToken)
        {
            var filteredUMAPendingRequests = LstData.AsQueryable();
            return InternalFind(filteredUMAPendingRequests, parameter);
        }

        public Task<SearchResult<UMAPendingRequest>> FindByOwner(string owner, SearchRequestParameter parameter, CancellationToken cancellationToken)
        {
            var filteredUMAPendingRequests = LstData.AsQueryable();
            filteredUMAPendingRequests = filteredUMAPendingRequests.Where(f => f.Owner == owner);
            return InternalFind(filteredUMAPendingRequests, parameter);
        }

        public Task<SearchResult<UMAPendingRequest>> FindByRequester(string requester, SearchRequestParameter parameter, CancellationToken cancellationToken)
        {
            var filteredUMAPendingRequests = LstData.AsQueryable();
            filteredUMAPendingRequests = filteredUMAPendingRequests.Where(f => f.Requester == requester);
            return InternalFind(filteredUMAPendingRequests, parameter);
        }

        public Task<SearchResult<UMAPendingRequest>> FindByResource(string resourceId, SearchRequestParameter parameter, CancellationToken cancellationToken)
        {
            var filteredUMAPendingRequests = LstData.AsQueryable();
            filteredUMAPendingRequests = filteredUMAPendingRequests.Where(f => f.Resource.Id == resourceId);
            return InternalFind(filteredUMAPendingRequests, parameter);
        }

        public Task<IEnumerable<UMAPendingRequest>> FindByTicketIdentifier(string ticketIdentifier, CancellationToken cancellationToken)
        {
            return Task.FromResult(LstData.Where(r => r.TicketId == ticketIdentifier));
        }

        public Task<UMAPendingRequest> FindByTicketIdentifierAndOwner(string ticketIdentifier, string owner, CancellationToken token)
        {
            return Task.FromResult(LstData.FirstOrDefault(r => r.TicketId == ticketIdentifier && r.Owner == owner));
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
