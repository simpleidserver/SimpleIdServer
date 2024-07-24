// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Stores;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Store.EF;

public class UserSessionRepository : IUserSessionResitory
{
    private readonly StoreDbContext _dbContext;

    public UserSessionRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<UserSession>> GetInactiveAndNotNotified(CancellationToken cancellationToken)
    {
        var currentDateTime = DateTime.UtcNow;
        var result = await _dbContext.UserSession
            .Include(s => s.User)
            .Where(s => (s.State == UserSessionStates.Rejected || s.ExpirationDateTime < currentDateTime) && s.IsClientsNotified == false)
            .ToListAsync(cancellationToken);
        return result;
    }

    public async Task<IEnumerable<UserSession>> GetActive(string userId, string realm, CancellationToken cancellationToken)
    {
        var currentDateTime = DateTime.UtcNow;
        var result = await _dbContext.UserSession.Where(s => s.UserId == userId && s.Realm == realm && s.State == UserSessionStates.Active && currentDateTime <= s.ExpirationDateTime).ToListAsync(cancellationToken);
        return result;
    }

    public async Task<UserSession> GetById(string sessionId, string realm, CancellationToken cancellationToken)
    {
        var session = await _dbContext.UserSession.FirstOrDefaultAsync(s => s.Realm == realm && s.SessionId == sessionId, cancellationToken);
        return session;
    }

    public async Task<SearchResult<UserSession>> Search(string userId, string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = _dbContext.UserSession
            .Where(u => u.Realm == realm && u.UserId == userId).AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        else
            query = query.OrderByDescending(u => u.AuthenticationDateTime);

        var count = query.Count();
        var users = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync(CancellationToken.None);
        return new SearchResult<UserSession>
        {
            Content = users,
            Count = count
        };
    }

    public void Add(UserSession session) => _dbContext.UserSession.Add(session);

    public void Update(UserSession session)
    {

    }
}
