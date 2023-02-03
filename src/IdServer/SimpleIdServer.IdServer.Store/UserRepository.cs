// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IUserRepository
    {
        IQueryable<User> Query();
        void Update(User user);
        void Add(User user);
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

        public void Update(User user) => _dbContext.Users.Update(user);

        public void Add(User user) => _dbContext.Users.Add(user);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
