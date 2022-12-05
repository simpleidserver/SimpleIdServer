// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Domains;

namespace SimpleIdServer.Store
{
    public interface IUserCommandRepository
    {
        IQueryable<User> Query();
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class UserCommandRepository : IUserCommandRepository
    {
        private readonly StoreCommandDbContext _dbContext;

        public UserCommandRepository(StoreCommandDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<User> Query() => _dbContext.Users;

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
