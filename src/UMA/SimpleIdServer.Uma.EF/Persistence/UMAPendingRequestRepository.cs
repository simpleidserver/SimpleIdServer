// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Uma.Domains;
using SimpleIdServer.Uma.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.EF.Persistence
{
    public class UMAPendingRequestRepository : IUMAPendingRequestRepository
    {
        private readonly UMAEFDbContext _dbContext;

        public UMAPendingRequestRepository(UMAEFDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> Add(UMAPendingRequest data, CancellationToken token)
        {
            _dbContext.PendingRequests.Add(data);
            return Task.FromResult(true);
        }

        public Task<bool> Delete(UMAPendingRequest data, CancellationToken token)
        {
            _dbContext.PendingRequests.Remove(data);
            return Task.FromResult(true);
        }

        public Task<bool> Update(UMAPendingRequest data, CancellationToken token)
        {
            _dbContext.PendingRequests.Update(data);
            return Task.FromResult(true);
        }

        public Task<int> SaveChanges(CancellationToken token)
        {
            return _dbContext.SaveChangesAsync(token);
        }
        public Task<SearchResult<UMAPendingRequest>> FindByOwner(string owner, SearchRequestParameter searchRequestParameter, CancellationToken cancellationToken)
        {
            var filtered = _dbContext.PendingRequests.Where(f => f.Owner == owner);
            return InternalFind(filtered, searchRequestParameter, cancellationToken);
        }

        public Task<SearchResult<UMAPendingRequest>> Find(SearchRequestParameter searchRequestParameter, CancellationToken cancellationToken)
        {
            return InternalFind(_dbContext.PendingRequests, searchRequestParameter, cancellationToken);
        }

        public Task<SearchResult<UMAPendingRequest>> FindByResource(string resourceId, SearchRequestParameter searchRequestParameter, CancellationToken cancellationToken)
        {
            var filtered = _dbContext.PendingRequests.Where(f => f.Resource.Id == resourceId);
            return InternalFind(filtered, searchRequestParameter, cancellationToken);
        }

        public Task<SearchResult<UMAPendingRequest>> FindByRequester(string requester, SearchRequestParameter searchRequestParameter, CancellationToken cancellationToken)
        {
            var filtered = _dbContext.PendingRequests.Where(f => f.Requester == requester);
            return InternalFind(filtered, searchRequestParameter, cancellationToken);
        }

        public async Task<IEnumerable<UMAPendingRequest>> FindByTicketIdentifier(string ticketIdentifier, CancellationToken cancellationToken)
        {
            IEnumerable<UMAPendingRequest> result = await _dbContext.PendingRequests.Where(r => r.TicketId == ticketIdentifier).ToListAsync(cancellationToken);
            return result;
        }

        public Task<UMAPendingRequest> FindByTicketIdentifierAndOwner(string ticketIdentifier, string owner, CancellationToken cancellationToken)
        {
            return _dbContext.PendingRequests.FirstOrDefaultAsync(r => r.TicketId == ticketIdentifier && r.Owner == owner, cancellationToken);
        }

        private async Task<SearchResult<UMAPendingRequest>> InternalFind(IQueryable<UMAPendingRequest> result, SearchRequestParameter parameter, CancellationToken cancellationToken)
        {
            var totalResults = await result.CountAsync(cancellationToken);
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

            var records = await result.Skip(parameter.StartIndex).Take(parameter.Count).ToListAsync(cancellationToken);
            return new SearchResult<UMAPendingRequest>
            {
                Records = records,
                TotalResults = totalResults
            };
        }

        public void Dispose()
        {
        }
    }
}
