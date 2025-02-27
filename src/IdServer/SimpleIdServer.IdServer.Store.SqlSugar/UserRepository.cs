// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class UserRepository : IUserRepository
{
    private readonly DbContext _dbContext;

    public UserRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(User user)
    {
        _dbContext.Client.InsertNav(SugarUser.Transform(user))
            .Include(u => u.Claims)
            .Include(u => u.Credentials)
            .Include(u => u.ExternalAuthProviders)
            .Include(u => u.Consents)
            .Include(u => u.Groups)
            .Include(u => u.Devices)
            .Include(u => u.Realms)
            .ExecuteCommand();
    }

    public async Task BulkUpdate(List<UserClaim> userClaims)
    {
        var claims = userClaims.Select(c => SugarUserClaim.Transform(c)).ToList();
        await _dbContext.Client.Updateable(claims).ExecuteCommandAsync();
    }

    public async Task BulkUpdate(List<User> users)
    {
        await _dbContext.Client.UpdateNav(users.Select(u => SugarUser.Transform(u)).ToList())
            .Include(u => u.Claims)
            .Include(u => u.Credentials)
            .Include(u => u.ExternalAuthProviders)
            .Include(u => u.Consents)
            .Include(u => u.Groups)
            .Include(u => u.Devices)
            .Include(u => u.Realms)
            .ExecuteCommandAsync();
    }

    public async Task BulkUpdate(List<RealmUser> userRealms)
    {
        await _dbContext.Client.Updateable(userRealms.Select(u => SugarRealmUser.Transform(u)).ToList()).ExecuteCommandAsync();
    }

    public async Task BulkUpdate(List<GroupUser> groupUsers)
    {
        await _dbContext.Client.Updateable(groupUsers.Select(u => SugarGroupUser.Transform(u)).ToList()).ExecuteCommandAsync();
    }

    public async Task<User> GetByClaim(string name, string value, string realm, CancellationToken cancellationToken)
    {
        var result = await GetUsers()
                    .FirstAsync(u => u.Realms.Any(r => r.RealmsName == realm) && u.Claims.Any(c => c.Name == name && c.Value == value), cancellationToken);
        return result?.ToDomain();
    }

    public async Task<User> GetByEmail(string email, string realm, CancellationToken cancellationToken)
    {
        var result = await GetUsers()
                    .FirstAsync(u => u.Email == email && u.Realms.Any(r => r.RealmsName == realm), cancellationToken);
        return result?.ToDomain();
    }

    public async Task<User> GetByExternalAuthProvider(string scheme, string sub, string realm, CancellationToken cancellationToken)
    {
        var result = await GetUsers()
                    .FirstAsync(u => u.ExternalAuthProviders.Any(e => e.Scheme == scheme && e.Subject == sub) && u.Realms.Any(r => r.RealmsName == realm), cancellationToken);
        return result?.ToDomain();
    }

    public async Task<User> GetById(string id, CancellationToken cancellationToken)
    {
        var result = await GetUsers()
                    .FirstAsync(u => u.Id == id, cancellationToken);
        return result?.ToDomain();
    }

    public async Task<User> GetById(string id, string realm, CancellationToken cancellationToken)
    {
        var result = await GetUsers()
                    .FirstAsync(u => u.Id == id && u.Realms.Any(r => r.RealmsName == realm), cancellationToken);
        return result?.ToDomain();
    }

    public async Task<User> GetBySubject(string subject, string realm, CancellationToken cancellationToken)
    {
        var result = await GetUsers()
                    .FirstAsync(u => u.Name == subject && u.Realms.Any(r => r.RealmsName == realm), cancellationToken);
        return result?.ToDomain();
    }

    public async Task<IEnumerable<User>> GetUsersById(IEnumerable<string> ids, string realm, CancellationToken cancellationToken)
    {
        var result = await GetUsers()
            .Where(u => u.Realms.Any(r => r.RealmsName == realm) && ids.Contains(u.Id))
            .ToListAsync(cancellationToken);
        return result.Select(u => u.ToDomain()).ToList();
    }

    public async Task<IEnumerable<User>> GetUsersBySubjects(IEnumerable<string> subjects, string realm, CancellationToken cancellationToken)
    {
        var users = await GetUsers()
            .Where(u => subjects.Contains(u.Name) && u.Realms.Any(r => r.RealmsName == realm))
            .ToListAsync(cancellationToken);
        return users.Select(u => u.ToDomain()).ToList();
    }

    public async Task<bool> IsClaimExists(string name, string value, string realm, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarUser>()
            .Includes(u => u.Realms)
            .Includes(u => u.Claims)
            .AnyAsync(u => u.Realms.Any(r => r.RealmsName == realm) && u.Claims.Any(c => c.Name == name && c.Value == value), cancellationToken);
        return result;
    }

    public async Task<bool> IsEmailExists(string email, string realm, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarUser>()
            .Includes(u => u.Realms)
            .AnyAsync(u => u.Realms.Any(r => r.RealmsName == realm) && u.Email == email, cancellationToken);
        return result;
    }

    public async Task<bool> IsExternalAuthProviderExists(string scheme, string sub, string realm, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarUser>()
            .Includes(u => u.Realms)
            .Includes(u => u.ExternalAuthProviders)
            .AnyAsync(u => u.Realms.Any(r => r.RealmsName == realm) && u.ExternalAuthProviders.Any(p => p.Subject == sub && p.Scheme == scheme), cancellationToken);
        return result;
    }

    public async Task<bool> IsSubjectExists(string sub, string realm, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarUser>()
            .Includes(u => u.Realms)
            .AnyAsync(u => u.Realms.Any(r => r.RealmsName == realm) && u.Name == sub, cancellationToken);
        return result;
    }

    public async Task<int> NbUsers(string realm, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarUser>()
            .Includes(u => u.Realms)
            .CountAsync(u => u.Realms.Any(r => r.RealmsName == realm), cancellationToken);
        return result;
    }

    public void Remove(IEnumerable<User> users)
    {
        _dbContext.Client.Deleteable(users.Select(u => SugarUser.Transform(u)).ToList()).ExecuteCommand();
    }

    public void Remove(IEnumerable<UserClaim> claims)
    {
        _dbContext.Client.Deleteable(claims.Select(u => SugarUserClaim.Transform(u)).ToList()).ExecuteCommand();
    }

    public async Task<SearchResult<User>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Client.Queryable<SugarUser>()
            .Includes(u => u.Realms)
            .Includes(u => u.Claims)
            .Where(u => u.Realms.Any(r => r.RealmsName == realm));
        /*
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        else
            query = query.OrderByDescending(u => u.UpdateDateTime);
        */
        query = query.OrderByDescending(u => u.UpdateDateTime);
        var count = query.Count();
        var users = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync(CancellationToken.None);
        return new SearchResult<User>
        {
            Content = users.Select(u => u.ToDomain()).ToList(),
            Count = count
        };
    }

    public void Update(User user)
    {
        _dbContext.Client.UpdateNav(SugarUser.Transform(user))
            .Include(u => u.Claims)
            .Include(u => u.Credentials)
            .Include(u => u.ExternalAuthProviders)
            .Include(u => u.Consents).ThenInclude(c => c.Scopes).ThenInclude(c => c.AuthorizedResources)
            .Include(u => u.Groups)
            .Include(u => u.Devices)
            .Include(u => u.Realms)
            .ExecuteCommand();
    }

    private ISugarQueryable<SugarUser> GetUsers() => _dbContext.Client.Queryable<SugarUser>()
                    .Includes(u => u.Consents, c => c.Scopes, c => c.AuthorizedResources)
                    .Includes(u => u.IdentityProvisioning, i => i.Definition)
                    .Includes(u => u.Credentials)
                    .Includes(u => u.ExternalAuthProviders)
                    .Includes(u => u.Groups)
                    .Includes(u => u.Devices)
                    .Includes(u => u.Claims)
                    .Includes(u => u.Realms);
}
