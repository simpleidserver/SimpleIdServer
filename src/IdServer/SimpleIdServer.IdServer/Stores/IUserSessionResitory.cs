// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IUserSessionResitory
{
    Task<IEnumerable<UserSession>> GetInactiveAndNotNotified(CancellationToken cancellationToken);
    Task<IEnumerable<UserSession>> GetActive(string userId, string realm, CancellationToken cancellationToken);
    Task<UserSession> GetById(string sessionId, string realm, CancellationToken cancellationToken);
    Task<int> NbActiveSessions(string realm, CancellationToken cancellationToken);
    Task<SearchResult<UserSession>> SearchActiveSessions(string realm, SearchRequest request, CancellationToken cancellationToken);
    Task<SearchResult<UserSession>> Search(string userId, string realm, SearchRequest request, CancellationToken cancellationToken);
    void Add(UserSession session);
    void Update(UserSession session);
}
