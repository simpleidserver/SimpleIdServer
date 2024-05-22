// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar
{
    internal class UserSessionRepository : IUserSessionResitory
    {
        public void Add(UserSession session)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserSession>> GetActive(string userId, string realm, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<UserSession> GetById(string sessionId, string realm, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserSession>> GetInactiveAndNotNotified(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IQueryable<UserSession> Query()
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<SearchResult<UserSession>> Search(string userId, string realm, SearchRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
