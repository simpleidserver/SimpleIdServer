// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IUserSessionResitory
    {
        IQueryable<UserSession> Query();
    }

    public class UserSessionRepository : IUserSessionResitory
    {
        private readonly StoreDbContext _dbContext;

        public UserSessionRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<UserSession> Query() => _dbContext.UserSession;
    }
}
