// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class UserSessionRepository : IUserSessionResitory
{
    private readonly DbContext _dbContext;

    public UserSessionRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(UserSession session)
    {   
        _dbContext.UserSessions.Insert(Transform(session));
    }

    public void Update(UserSession session)
    {
        _dbContext.Client.Updateable(SugarUserSession.Transform(session))
            .ExecuteCommand();
    }

    public async Task<IEnumerable<UserSession>> GetActive(string userId, string realm, CancellationToken cancellationToken)
    {
        var currentDateTime = DateTime.UtcNow;
        var result = await _dbContext.Client.Queryable<SugarUserSession>()
            .Where(s => s.UserId == userId && s.Realm == realm && s.State == UserSessionStates.Active && currentDateTime <= s.ExpirationDateTime)
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain());
    }

    public async Task<UserSession> GetById(string sessionId, string realm, CancellationToken cancellationToken)
    {
        var session = await _dbContext.Client.Queryable<SugarUserSession>()
            .FirstAsync(s => s.Realm == realm && s.SessionId == sessionId, cancellationToken);
        return session?.ToDomain();
    }

    public async Task<IEnumerable<UserSession>> GetInactiveAndNotNotified(CancellationToken cancellationToken)
    {
        var currentDateTime = DateTime.UtcNow;
        var session = await _dbContext.Client.Queryable<SugarUserSession>()
            .Includes(s => s.User)
            .Where(s => (s.State == UserSessionStates.Rejected || s.ExpirationDateTime < currentDateTime) && s.IsClientsNotified == false)
            .ToListAsync(cancellationToken);
        return session.Select(s => s.ToDomain());
    }

    public async Task<SearchResult<UserSession>> Search(string userId, string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Client.Queryable<SugarUserSession>()
            .Where(u => u.Realm == realm && u.UserId == userId);
        /*
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        else
            query = query.OrderByDescending(u => u.AuthenticationDateTime);
        */
        query = query.OrderByDescending(s => s.SessionId);

        var count = query.Count();
        var users = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync(CancellationToken.None);
        return new SearchResult<UserSession>
        {
            Content = users.Select(u => u.ToDomain()).ToList(),
            Count = count
        };
    }

    private static SugarUserSession Transform(UserSession userSession)
    {
        return new SugarUserSession
        {
            AuthenticationDateTime = userSession.AuthenticationDateTime,
            ExpirationDateTime = userSession.ExpirationDateTime,
            IsClientsNotified = userSession.IsClientsNotified,
            SerializedClientIds = userSession.SerializedClientIds,
            SessionId = userSession.SessionId,
            State = userSession.State,
            UserId = userSession.UserId,
            Realm = userSession.Realm
        };
    }
}
