// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using EFCore.BulkExtensions;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IUserRepository
    {
        Task<User> Get(Func<IQueryable<User>, Task<User>> callback);
        Task<IEnumerable<User>> GetAll(Func<IQueryable<User>, Task<List<User>>> callback);
        IQueryable<User> Query();
        void Update(User user);
        void Add(User user);
        void Remove(IEnumerable<User> users);
        Task BulkUpdate(List<UserClaim> userClaims);
        Task BulkUpdate(List<User> users);
        Task BulkUpdate(List<RealmUser> userRealms);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class UserRepository : IUserRepository
    {
        private readonly StoreDbContext _dbContext;

        public UserRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async virtual Task<User> Get(Func<IQueryable<User>, Task<User>> callback)
        {
            var user = await callback(_dbContext.Users);
            return user;
        }

        public async virtual Task<IEnumerable<User>> GetAll(Func<IQueryable<User>, Task<List<User>>> callback)
        {
            var users = await callback(_dbContext.Users);
            return users;
        }

        public IQueryable<User> Query() => _dbContext.Users;

        public void Update(User user) => _dbContext.Users.Update(user);

        public void Add(User user) => _dbContext.Users.Add(user);

        public void Remove(IEnumerable<User> users) => _dbContext.Users.RemoveRange(users);

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

        public Task BulkUpdate(List<RealmUser> userRealms)
        {
            var bulkConfig = new BulkConfig
            {
                PropertiesToIncludeOnCompare = new List<string> { nameof(RealmUser.RealmsName), nameof(RealmUser.UsersId) }
            };
            return _dbContext.BulkInsertOrUpdateAsync(userRealms, bulkConfig);
        }

        public virtual Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
