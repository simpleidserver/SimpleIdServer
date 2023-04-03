// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using EFCore.BulkExtensions;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IUserRepository
    {
        IQueryable<User> Query();
        void Update(User user);
        void Add(User user);
        Task BulkUpdate(List<UserClaim> userClaims);
        Task BulkUpdate(List<User> users);
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

        public Task BulkUpdate(List<UserClaim> userClaims)
        {
            var bulkConfig = new BulkConfig
            {
                PropertiesToIncludeOnCompare = new List<string> { nameof(UserClaim.Id), nameof(UserClaim.UserId), nameof(UserClaim.Name), nameof(UserClaim.Value) }
            };
            return _dbContext.BulkInsertOrUpdateAsync(userClaims, bulkConfig);
        }

        public Task BulkUpdate(List<User> users)
        {
            var bulkConfig = new BulkConfig
            {
                PropertiesToIncludeOnCompare = new List<string> { nameof(User.Id), nameof(User.Name), nameof(User.Firstname), nameof(User.Lastname), nameof(User.Email), nameof(User.EmailVerified) }
            };
            return _dbContext.BulkInsertOrUpdateAsync(users, bulkConfig);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
