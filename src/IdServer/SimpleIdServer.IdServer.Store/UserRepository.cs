// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Store
{
    public interface IUserRepository
    {
        Task<User> GetBySubject(string subject, string realm, CancellationToken cancellationToken);
        Task<User> GetById(string id, string realm, CancellationToken cancellationToken);
        Task<User> GetByEmail(string email, string realm, CancellationToken cancellationToken);
        Task<User> GetByExternalAuthProvider(string scheme, string sub, string realm, CancellationToken cancellationToken);
        Task<User> GetByClaim(string name, string value, string realm, CancellationToken cancellationToken);
        Task<IEnumerable<User>> GetUsersById(IEnumerable<string> ids, string realm, CancellationToken cancellationToken);
        Task<IEnumerable<User>> GetUsersBySubjects(IEnumerable<string> subjects, string realm, CancellationToken cancellationToken);
        Task<int> NbUsers(string realm, CancellationToken cancellationToken);
        Task<bool> IsExternalAuthProviderExists(string scheme, string sub, string realm, CancellationToken cancellationToken);
        Task<bool> IsSubjectExists(string sub, string realm, CancellationToken cancellationToken);
        Task<bool> IsEmailExists(string email, string realm, CancellationToken cancellationToken);
        Task<bool> IsClaimExists(string name, string value, string realm, CancellationToken cancellationToken);
        Task<SearchResult<User>> Search(string realm, SearchRequest request, CancellationToken cancellationToken);
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

        public async virtual Task<User> GetBySubject(string subject, string realm, CancellationToken cancellationToken)
        {
            var result = await GetUsers()
                        .FirstOrDefaultAsync(u => u.Name == subject && u.Realms.Any(r => r.RealmsName == realm), cancellationToken);
            return result;
        }

        public async virtual Task<User> GetById(string id, string realm, CancellationToken cancellationToken)
        {
            var result = await GetUsers()
                        .FirstOrDefaultAsync(u => u.Id == id && u.Realms.Any(r => r.RealmsName == realm), cancellationToken);
            return result;
        }
        
        public async virtual Task<User> GetByEmail(string email, string realm, CancellationToken cancellationToken)
        {
            var result = await GetUsers()
                        .FirstOrDefaultAsync(u => u.Email == email && u.Realms.Any(r => r.RealmsName == realm), cancellationToken);
            return result;
        }

        public async virtual Task<User> GetByExternalAuthProvider(string scheme, string sub, string realm, CancellationToken cancellationToken)
        {
            var result = await GetUsers()
                        .FirstOrDefaultAsync(u => u.ExternalAuthProviders.Any(e => e.Scheme == scheme && e.Subject == sub) && u.Realms.Any(r => r.RealmsName == realm), cancellationToken);
            return result;
        }

        public async virtual Task<User> GetByClaim(string name, string value, string realm, CancellationToken cancellationToken)
        {
            var result = await GetUsers()
                        .FirstOrDefaultAsync(u => u.Realms.Any(r => r.RealmsName == realm) && u.OAuthUserClaims.Any(c => c.Name == name && c.Value == value), cancellationToken);
            return result;
        }

        public async virtual Task<IEnumerable<User>> GetUsersById(IEnumerable<string> ids, string realm, CancellationToken cancellationToken)
        {
            var result = await GetUsers()
                .Where(u => u.Realms.Any(r => r.RealmsName == realm) && ids.Contains(u.Id))
                .ToListAsync(cancellationToken);
            return result;
        }

        public async virtual Task<IEnumerable<User>> GetUsersBySubjects(IEnumerable<string> subjects, string realm, CancellationToken cancellationToken)
        {
            var users = await GetUsers()
                .Where(u => subjects.Contains(u.Name) && u.Realms.Any(r => r.RealmsName == realm))
                .ToListAsync(cancellationToken);
            return users;
        }

        public async Task<int> NbUsers(string realm, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Users
                .Include(u => u.Realms)
                .AsNoTracking()
                .CountAsync(u => u.Realms.Any(r => r.RealmsName == realm), cancellationToken);
            return result;
        }

        public async virtual Task<bool> IsExternalAuthProviderExists(string scheme, string sub, string realm, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Users
                .Include(u => u.Realms)
                .Include(u => u.ExternalAuthProviders)
                .AsNoTracking()
                .AnyAsync(u => u.Realms.Any(r => r.RealmsName == realm) && u.ExternalAuthProviders.Any(p => p.Subject == sub && p.Scheme == scheme), cancellationToken);
            return result;
        }

        public async virtual Task<bool> IsSubjectExists(string sub, string realm, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Users
                .Include(u => u.Realms)
                .AsNoTracking()
                .AnyAsync(u => u.Realms.Any(r => r.RealmsName == realm) && u.Name == sub, cancellationToken);
            return result;
        }

        public async virtual Task<bool> IsEmailExists(string email, string realm, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Users
                .Include(u => u.Realms)
                .AsNoTracking()
                .AnyAsync(u => u.Realms.Any(r => r.RealmsName == realm) && u.Email == email, cancellationToken);
            return result;
        }

        public async virtual Task<bool> IsClaimExists(string name, string value, string realm, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Users
                .Include(u => u.Realms)
                .Include(u => u.OAuthUserClaims)
                .AsNoTracking()
                .AnyAsync(u => u.Realms.Any(r => r.RealmsName == realm) && u.OAuthUserClaims.Any(c => c.Name == name && c.Value == value), cancellationToken);
            return result;
        }

        public async virtual Task<SearchResult<User>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Users
                .Include(u => u.Realms)
                .Include(u => u.OAuthUserClaims)
                .Where(u => u.Realms.Any(r => r.RealmsName == realm)).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(request.Filter))
                query = query.Where(request.Filter);

            if (!string.IsNullOrWhiteSpace(request.OrderBy))
                query = query.OrderBy(request.OrderBy);
            else
                query = query.OrderByDescending(u => u.UpdateDateTime);

            var count = query.Count();
            var users = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync(CancellationToken.None);
            return new SearchResult<User>
            {
                Content = users,
                Count = count
            };
        }

        public async virtual Task<IEnumerable<User>> GetAll(Func<IQueryable<User>, Task<List<User>>> callback)
        {
            var users = await callback(_dbContext.Users);
            return users;
        }

        public virtual void Update(User user) => _dbContext.Users.Update(user);

        public virtual void Add(User user) => _dbContext.Users.Add(user);

        public void Remove(IEnumerable<User> users) => _dbContext.Users.RemoveRange(users);

        public virtual async Task BulkUpdate(List<UserClaim> userClaims)
        {
            if (_dbContext.Database.IsRelational())
            {
                var bulkConfig = new BulkConfig
                {
                    PropertiesToIncludeOnCompare = new List<string> { nameof(UserClaim.Id), nameof(UserClaim.UserId), nameof(UserClaim.Name), nameof(UserClaim.Value) }
                };
                await _dbContext.BulkInsertOrUpdateAsync(userClaims, bulkConfig);
                return;
            }

            var userIds = userClaims.Select(u => u.UserId).ToList();
            var existingUsers = await _dbContext.Users
                .Include(u => u.OAuthUserClaims)
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();
            foreach(var existingUser in existingUsers)
            {
                var newClaims = existingUser.OAuthUserClaims.Where(uc => !userClaims.Any(c => c.Name == uc.Name)).ToList();
                newClaims.AddRange(userClaims);
                existingUser.OAuthUserClaims = newClaims;
            }

            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task BulkUpdate(List<User> users)
        {
            if(_dbContext.Database.IsRelational())
            {
                var bulkConfig = new BulkConfig
                {
                    PropertiesToIncludeOnCompare = new List<string> { nameof(User.Id), nameof(User.Name), nameof(User.Firstname), nameof(User.Lastname), nameof(User.Email), nameof(User.EmailVerified) }
                };
                await _dbContext.BulkInsertOrUpdateAsync(users, bulkConfig);
                return;
            }

            var userIds = users.Select(u => u.Id).ToList();
            var existingUsers = await _dbContext.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
            _dbContext.Users.RemoveRange(existingUsers);
            _dbContext.Users.AddRange(users);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task BulkUpdate(List<RealmUser> userRealms)
        {
            if (_dbContext.Database.IsRelational())
            {
                var bulkConfig = new BulkConfig
                {
                    PropertiesToIncludeOnCompare = new List<string> { nameof(RealmUser.RealmsName), nameof(RealmUser.UsersId) }
                };
                await _dbContext.BulkInsertOrUpdateAsync(userRealms, bulkConfig);
            }

            var userIds = userRealms.Select(r => r.UsersId).ToList();
            var existingUsers = await _dbContext.Users
                .Include(u => u.Realms)
                .Where(u => userIds.Contains(u.Id)).ToListAsync();
            foreach(var existingUser in existingUsers)
            {
                existingUser.Realms = userRealms.Where(r => r.UsersId == existingUser.Id).ToList();
            }

            await _dbContext.SaveChangesAsync();
        }

        public virtual Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);

        private IQueryable<User> GetUsers() => _dbContext.Users
                        .Include(u => u.Consents).ThenInclude(c => c.Scopes).ThenInclude(c => c.AuthorizedResources)
                        .Include(u => u.IdentityProvisioning).ThenInclude(i => i.Definition)
                        .Include(u => u.Credentials)
                        .Include(u => u.ExternalAuthProviders)
                        .Include(u => u.Groups)
                        .Include(u => u.Devices)
                        .Include(u => u.OAuthUserClaims)
                        .Include(u => u.CredentialOffers)
                        .Include(u => u.Realms);
    }
}
