// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultUserSessionRepository : IUserSessionResitory
{
    private readonly List<UserSession> _userSessions;

    public DefaultUserSessionRepository(List<UserSession> userSessions)
    {
        _userSessions = userSessions;
    }

    public Task<IEnumerable<UserSession>> GetInactiveAndNotNotified(CancellationToken cancellationToken)
    {
        var currentDateTime = DateTime.UtcNow;
        var result = _userSessions
            .Where(s => (s.State == UserSessionStates.Rejected || s.ExpirationDateTime < currentDateTime) && !s.IsClientsNotified)
            .ToList()
            .AsEnumerable();
        return Task.FromResult(result);
    }

    public Task<IEnumerable<UserSession>> GetActive(string userId, string realm, CancellationToken cancellationToken)
    {
        var currentDateTime = DateTime.UtcNow;
        var result = _userSessions
            .Where(s => s.UserId == userId && s.Realm == realm && s.State == UserSessionStates.Active && currentDateTime <= s.ExpirationDateTime)
            .ToList()
            .AsEnumerable();
        return Task.FromResult(result);
    }

    public Task<UserSession> GetById(string sessionId, string realm, CancellationToken cancellationToken)
    {
        var session = _userSessions.FirstOrDefault(s => s.Realm == realm && s.SessionId == sessionId);
        return Task.FromResult(session);
    }

    public Task<int> NbActiveSessions(string realm, CancellationToken cancellationToken)
    {
        var count = _userSessions.Count(s => s.Realm == realm && s.State == UserSessionStates.Active);
        return Task.FromResult(count);
    }

    public Task<SearchResult<UserSession>> SearchActiveSessions(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = _userSessions.AsQueryable()
            .Where(u => u.Realm == realm && u.State == UserSessionStates.Active)
            .OrderBy(u => u.SessionId);
        var count = query.Count();
        var sessions = query.Skip(request.Skip.Value).Take(request.Take.Value).ToList();
        var result = new SearchResult<UserSession>
        {
            Content = sessions,
            Count = count
        };
        return Task.FromResult(result);
    }

    public Task<SearchResult<UserSession>> Search(string userId, string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = _userSessions.AsQueryable().Where(u => u.Realm == realm && u.UserId == userId);
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        else
            query = query.OrderByDescending(u => u.AuthenticationDateTime);
        var count = query.Count();
        var sessions = query.Skip(request.Skip.Value).Take(request.Take.Value).ToList();
        var result = new SearchResult<UserSession>
        {
            Content = sessions,
            Count = count
        };
        return Task.FromResult(result);
    }

    public void Add(UserSession session) => _userSessions.Add(session);

    public void Update(UserSession session)
    {
        var existing = _userSessions.FirstOrDefault(s => s.SessionId == session.SessionId && s.Realm == session.Realm);
        if (existing != null)
        {
            var index = _userSessions.IndexOf(existing);
            _userSessions[index] = session;
        }
    }
}
