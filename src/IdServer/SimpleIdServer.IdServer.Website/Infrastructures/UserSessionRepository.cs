// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Caching.Distributed;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Website.Infrastructures;

public class UserSessionRepository : IUserSessionResitory
{
    private readonly IDistributedCache _distributedCache;

    public UserSessionRepository(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public void Add(UserSession session)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserSession>> GetActive(string userId, string realm, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<UserSession> GetById(string sessionId, string realm, CancellationToken cancellationToken)
    {
        var cacheValue = await _distributedCache.GetStringAsync(sessionId);
        if (cacheValue == null) return new UserSession
        {
            ExpirationDateTime = DateTime.UtcNow.AddDays(2)
        };
        await _distributedCache.RemoveAsync(sessionId);
        return null;
    }

    public Task<IEnumerable<UserSession>> GetInactiveAndNotNotified(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<int> NbActiveSessions(string realm, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<SearchResult<UserSession>> Search(string userId, string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<SearchResult<UserSession>> SearchActiveSessions(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Update(UserSession session)
    {
        throw new NotImplementedException();
    }
}
