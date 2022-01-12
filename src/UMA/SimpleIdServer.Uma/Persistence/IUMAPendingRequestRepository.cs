// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.Uma.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Persistence
{
    public interface IUMAPendingRequestRepository : ICommandRepository<UMAPendingRequest>
    {
        Task<SearchResult<UMAPendingRequest>> Find(SearchRequestParameter searchRequestParameter, CancellationToken cancellationToken);
        Task<SearchResult<UMAPendingRequest>> FindByOwner(string owner, SearchRequestParameter searchRequestParameter, CancellationToken cancellationToken);
        Task<SearchResult<UMAPendingRequest>> FindByRequester(string requester, SearchRequestParameter searchRequestParameter, CancellationToken cancellationToken);
        Task<SearchResult<UMAPendingRequest>> FindByResource(string resourceId, SearchRequestParameter searchRequestParameter, CancellationToken cancellationToken);
        Task<IEnumerable<UMAPendingRequest>> FindByTicketIdentifier(string ticketIdentifier, CancellationToken cancellationToken);
        Task<UMAPendingRequest> FindByTicketIdentifierAndOwner(string ticketIdentifier, string owner, CancellationToken cancellationToken);
    }
}
