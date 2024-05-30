// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Store.EF;

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

    public async Task<User> GetById(string id, CancellationToken cancellationToken)
    {
        var result = await GetUsers()
                    .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
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
        if(_dbContext.Database.IsRelational())
        {
            var merged = LinqToDB.LinqExtensions.UpdateWhenMatched(
                        LinqToDB.LinqExtensions.InsertWhenNotMatched(
                            LinqToDB.LinqExtensions.On(
                                LinqToDB.LinqExtensions.Using(
                                    LinqToDB.LinqExtensions.Merge(
                                        _dbContext.UserClaims.ToLinqToDBTable()),
                                        userClaims
                                    ),
                                    (g1, g2) => g1.Id == g2.Id
                            ),
                            source => source),
                        (target, source) => new UserClaim
                        {
                            Name = source.Name,
                            Value = source.Value,
                            UserId = source.UserId
                        });
            LinqToDB.LinqExtensions.Merge(merged);
            return;
        }

        var userClaimIds = userClaims.Select(u => u.Id).ToList();
        var existingUserClaims = await _dbContext.UserClaims
            .Where(u => userClaimIds.Contains(u.Id))
            .ToListAsync();
        _dbContext.UserClaims.RemoveRange(existingUserClaims);
        _dbContext.UserClaims.AddRange(userClaims);
        await _dbContext.SaveChangesAsync();
    }

    public virtual async Task BulkUpdate(List<User> users)
    {
        if(_dbContext.Database.IsRelational())
        {
            var merged = LinqToDB.LinqExtensions.UpdateWhenMatched(
                        LinqToDB.LinqExtensions.InsertWhenNotMatched(
                            LinqToDB.LinqExtensions.On(
                                LinqToDB.LinqExtensions.Using(
                                    LinqToDB.LinqExtensions.Merge(
                                        _dbContext.Users.ToLinqToDBTable()),
                                        users
                                    ),
                                    (g1, g2) => g1.Id == g2.Id
                            ),
                            source => source),
                        (target, source) => new User
                        {
                            Name = source.Name,
                            Firstname = source.Firstname,
                            Lastname = source.Lastname,
                            Email = source.Email,
                            EmailVerified = source.EmailVerified
                        });
            LinqToDB.LinqExtensions.Merge(merged);
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
            var merged = LinqToDB.LinqExtensions.UpdateWhenMatched(
                        LinqToDB.LinqExtensions.InsertWhenNotMatched(
                            LinqToDB.LinqExtensions.On(
                                LinqToDB.LinqExtensions.Using(
                                    LinqToDB.LinqExtensions.Merge(
                                        _dbContext.RealmUser.ToLinqToDBTable()),
                                        userRealms
                                    ),
                                    (g1, g2) => g1.RealmsName == g2.RealmsName && g1.UsersId == g2.UsersId
                            ),
                            source => source),
                        (target, source) => new RealmUser
                        {
                            RealmsName = source.RealmsName,
                            UsersId = source.UsersId
                        });
            LinqToDB.LinqExtensions.Merge(merged);
            return;
        }

        var userIds = userRealms.Select(r => r.UsersId).ToList();
        var existingRealms = await _dbContext.RealmUser
            .Where(u => userIds.Contains(u.UsersId))
            .ToListAsync();
        var newRealms = userRealms.Where(g => !existingRealms.Any(r => r.UsersId == g.UsersId && r.RealmsName == g.RealmsName));
        _dbContext.RealmUser.AddRange(newRealms);
        await _dbContext.SaveChangesAsync();
    }

    public async Task BulkUpdate(List<GroupUser> groupUsers)
    {
        if (_dbContext.Database.IsRelational())
        {
            var merged = LinqToDB.LinqExtensions.UpdateWhenMatched(
                        LinqToDB.LinqExtensions.InsertWhenNotMatched(
                            LinqToDB.LinqExtensions.On(
                                LinqToDB.LinqExtensions.Using(
                                    LinqToDB.LinqExtensions.Merge(
                                        _dbContext.GroupUser.ToLinqToDBTable()),
                                        groupUsers
                                    ),
                                    (g1, g2) => g1.GroupsId == g2.GroupsId && g1.UsersId == g2.UsersId
                            ),
                            source => source),
                        (target, source) => new GroupUser
                        {
                            GroupsId = source.GroupsId,
                            UsersId = source.UsersId
                        });
            LinqToDB.LinqExtensions.Merge(merged);
            return;
        }

        var userIds = groupUsers.Select(r => r.UsersId).ToList();
        var existingGroupUsers = await _dbContext.GroupUser
            .Where(u => userIds.Contains(u.UsersId))
            .ToListAsync();
        var newGroupUsers = groupUsers.Where(g => !existingGroupUsers.Any(eg => eg.UsersId == g.UsersId && eg.GroupsId == g.GroupsId));
        _dbContext.GroupUser.AddRange(newGroupUsers);
        await _dbContext.SaveChangesAsync();
    }

    private IQueryable<User> GetUsers() => _dbContext.Users
                    .Include(u => u.Consents).ThenInclude(c => c.Scopes).ThenInclude(c => c.AuthorizedResources)
                    .Include(u => u.IdentityProvisioning).ThenInclude(i => i.Definition)
                    .Include(u => u.Credentials)
                    .Include(u => u.ExternalAuthProviders)
                    .Include(u => u.Groups).ThenInclude(u => u.Group)
                    .Include(u => u.Devices)
                    .Include(u => u.OAuthUserClaims)
                    .Include(u => u.Realms);
}
