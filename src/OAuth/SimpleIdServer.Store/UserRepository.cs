// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Domains;

namespace SimpleIdServer.Store
{
    public interface IUserRepository
    {
        IQueryable<User> Query();
        void Update(User user);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class UserRepository : IUserRepository
    {
        private readonly StoreDbContext _dbContext;

        public UserRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<User> Query() => _dbContext.Users;

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);

        public void Update(User user)
        {
            var entry = _dbContext.Entry(user);
            if (entry.State != EntityState.Modified)
                _dbContext.Entry(entry).State = EntityState.Modified;
        }
    }
}
