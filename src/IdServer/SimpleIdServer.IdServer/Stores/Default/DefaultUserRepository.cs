// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultUserRepository : IUserRepository
{
    private List<User> _users { get; }
    private IQueryable<User> UsersQueryable => _users.AsQueryable();

    public DefaultUserRepository(List<User> users) => _users = users;

    public virtual Task<User> GetBySubject(string subject, string realm, CancellationToken cancellationToken)
    {
        using(var activity = Tracing.UserActivitySource.StartActivity("GetUserBySubject"))
        {
            activity?.SetTag(Tracing.UserTagNames.Id, subject);
            activity?.SetTag(Tracing.CommonTagNames.Realm, realm);
            var result = UsersQueryable.FirstOrDefault(u => u.Name == subject && u.Realms.Any(r => r.RealmsName == realm));
            return Task.FromResult(result);
        }
    }

    public Task<User> GetById(string id, CancellationToken cancellationToken)
    {
        var result = UsersQueryable.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(result);
    }

    public virtual Task<User> GetById(string id, string realm, CancellationToken cancellationToken)
    {
        var result = UsersQueryable.FirstOrDefault(u => u.Id == id && u.Realms.Any(r => r.RealmsName == realm));
        return Task.FromResult(result);
    }

    public virtual Task<User> GetByEmail(string email, string realm, CancellationToken cancellationToken)
    {
        var result = UsersQueryable.FirstOrDefault(u => u.Email == email && u.Realms.Any(r => r.RealmsName == realm));
        return Task.FromResult(result);
    }

    public virtual Task<User> GetByExternalAuthProvider(string scheme, string sub, string realm, CancellationToken cancellationToken)
    {
        var result = UsersQueryable.FirstOrDefault(u => u.ExternalAuthProviders.Any(e => e.Scheme == scheme && e.Subject == sub) && u.Realms.Any(r => r.RealmsName == realm));
        return Task.FromResult(result);
    }

    public virtual Task<User> GetByClaim(string name, string value, string realm, CancellationToken cancellationToken)
    {
        var result = UsersQueryable.FirstOrDefault(u => u.Realms.Any(r => r.RealmsName == realm) && u.OAuthUserClaims.Any(c => c.Name == name && c.Value == value));
        return Task.FromResult(result);
    }

    public virtual Task<IEnumerable<User>> GetUsersById(IEnumerable<string> ids, string realm, CancellationToken cancellationToken)
    {
        var result = UsersQueryable.Where(u => u.Realms.Any(r => r.RealmsName == realm) && ids.Contains(u.Id)).ToList();
        return Task.FromResult((IEnumerable<User>)result);
    }

    public virtual Task<IEnumerable<User>> GetUsersBySubjects(IEnumerable<string> subjects, string realm, CancellationToken cancellationToken)
    {
        var users = UsersQueryable.Where(u => subjects.Contains(u.Name) && u.Realms.Any(r => r.RealmsName == realm)).ToList();
        return Task.FromResult((IEnumerable<User>)users);
    }

    public Task<int> NbUsers(string realm, CancellationToken cancellationToken)
    {
        var count = _users.Count(u => u.Realms.Any(r => r.RealmsName == realm));
        return Task.FromResult(count);
    }

    public virtual Task<bool> IsExternalAuthProviderExists(string scheme, string sub, string realm, CancellationToken cancellationToken)
    {
        var result = UsersQueryable.Any(u => u.Realms.Any(r => r.RealmsName == realm) && u.ExternalAuthProviders.Any(p => p.Subject == sub && p.Scheme == scheme));
        return Task.FromResult(result);
    }

    public virtual Task<bool> IsSubjectExists(string sub, string realm, CancellationToken cancellationToken)
    {
        var result = UsersQueryable.Any(u => u.Realms.Any(r => r.RealmsName == realm) && u.Name == sub);
        return Task.FromResult(result);
    }

    public virtual Task<bool> IsEmailExists(string email, string realm, CancellationToken cancellationToken)
    {
        var result = UsersQueryable.Any(u => u.Realms.Any(r => r.RealmsName == realm) && u.Email == email);
        return Task.FromResult(result);
    }

    public virtual Task<bool> IsClaimExists(string name, string value, string realm, CancellationToken cancellationToken)
    {
        var result = UsersQueryable.Any(u => u.Realms.Any(r => r.RealmsName == realm) && u.OAuthUserClaims.Any(c => c.Name == name && c.Value == value));
        return Task.FromResult(result);
    }

    public virtual Task<SearchResult<User>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = _users.AsQueryable().Where(u => u.Realms.Any(r => r.RealmsName == realm));
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);
        query = !string.IsNullOrWhiteSpace(request.OrderBy) ? query.OrderBy(request.OrderBy) : query.OrderByDescending(u => u.UpdateDateTime);
        var count = query.Count();
        var users = query.Skip(request.Skip.Value).Take(request.Take.Value).ToList();
        var result = new SearchResult<User>
        {
            Content = users,
            Count = count
        };
        return Task.FromResult(result);
    }

    public virtual Task<IEnumerable<User>> GetAll(Func<IQueryable<User>, Task<List<User>>> callback)
    {
        return callback(_users.AsQueryable()).ContinueWith(t => (IEnumerable<User>)t.Result);
    }

    public virtual void Update(User user)
    {
        var index = _users.FindIndex(u => u.Id == user.Id);
        if (index >= 0)
            _users[index] = user;
    }

    public virtual void Add(User user) => _users.Add(user);

    public void Remove(IEnumerable<User> users)
    {
        foreach (var user in users)
            _users.Remove(user);
    }

    public void Remove(IEnumerable<UserClaim> claims)
    {
        foreach (var user in _users)
            user.OAuthUserClaims = user.OAuthUserClaims.Where(c => !claims.Any(rc => rc.Id == c.Id)).ToList();
    }

    public virtual Task BulkUpdate(List<UserClaim> userClaims)
    {
        foreach (var claim in userClaims)
        {
            var user = _users.FirstOrDefault(u => u.Id == claim.GetType().GetProperty("UserId")?.GetValue(claim)?.ToString());
            if (user != null)
            {
                user.OAuthUserClaims = user.OAuthUserClaims.Where(c => c.Id != claim.Id).ToList();
                user.OAuthUserClaims.Add(claim);
            }
        }
        return Task.CompletedTask;
    }

    public virtual Task BulkUpdate(List<User> users)
    {
        foreach (var user in users)
        {
            var existingIndex = _users.FindIndex(u => u.Id == user.Id);
            if (existingIndex >= 0)
                _users[existingIndex] = user;
            else
                _users.Add(user);
        }
        return Task.CompletedTask;
    }

    public virtual Task BulkUpdate(List<RealmUser> userRealms)
    {
        foreach (var realmUser in userRealms)
        {
            var user = _users.FirstOrDefault(u => u.Id == realmUser.UsersId);
            if (user != null && !user.Realms.Any(r => r.RealmsName == realmUser.RealmsName))
                user.Realms.Add(realmUser);
        }
        return Task.CompletedTask;
    }

    public Task BulkUpdate(List<GroupUser> groupUsers)
    {
        foreach (var groupUser in groupUsers)
        {
            var user = _users.FirstOrDefault(u => u.Id == groupUser.UsersId);
            if (user != null && !user.Groups.Any(g => g.GroupsId == groupUser.GroupsId))
                user.Groups.Add(groupUser);
        }
        return Task.CompletedTask;
    }
}
