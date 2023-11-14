using EFCore.BulkExtensions;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UseOpenIddictAsDatasource.Services
{
    public class CustomUserRepository : IUserRepository
    {
        private readonly StoreDbContext _dbContext;
        private readonly ApplicationDbContext _appDbContext;

        public CustomUserRepository(StoreDbContext dbContext, ApplicationDbContext appDbContext)
        {
            _dbContext = dbContext;
            _appDbContext = appDbContext;
        }

        public virtual async Task<User> Get(Func<IQueryable<User>, Task<User>> callback)
        {
            return await callback(_dbContext.Users);
        }

        public virtual async Task<IEnumerable<User>> GetAll(Func<IQueryable<User>, Task<List<User>>> callback)
        {
            return await callback(_dbContext.Users);
        }

        public IQueryable<User> Query()
        {
            return _dbContext.Users;
        }

        public void Update(User user)
        {
            _dbContext.Users.Update(user);
        }

        public void Add(User user)
        {
            _dbContext.Users.Add(user);
            _appDbContext.Users.Add(new ApplicationUser
            {
                Id = user.Id,
                Email = user.Email
            });
        }

        public void Remove(IEnumerable<User> users)
        {
            _dbContext.Users.RemoveRange(users);
        }

        public Task BulkUpdate(List<UserClaim> userClaims)
        {
            BulkConfig bulkConfig = new BulkConfig
            {
                PropertiesToIncludeOnCompare = new List<string> { "Id", "UserId", "Name", "Value" }
            };
            return _dbContext.BulkInsertOrUpdateAsync(userClaims, bulkConfig);
        }

        public Task BulkUpdate(List<User> users)
        {
            BulkConfig bulkConfig = new BulkConfig
            {
                PropertiesToIncludeOnCompare = new List<string> { "Id", "Name", "Firstname", "Lastname", "Email", "EmailVerified" }
            };
            return _dbContext.BulkInsertOrUpdateAsync(users, bulkConfig);
        }

        public Task BulkUpdate(List<RealmUser> userRealms)
        {
            BulkConfig bulkConfig = new BulkConfig
            {
                PropertiesToIncludeOnCompare = new List<string> { "RealmsName", "UsersId" }
            };
            return _dbContext.BulkInsertOrUpdateAsync(userRealms, bulkConfig);
        }

        public virtual async Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            return 1;
        }
    }
}
