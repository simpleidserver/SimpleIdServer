// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Api.Groups;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class GroupRepository : IGroupRepository
{
    private readonly DbContext _dbContext;

    public GroupRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Group group)
    {
        _dbContext.Client.InsertNav(Transform(group))
            .Include(g => g.Realms)
            .ExecuteCommand();
    }

    public void Update(Group group)
    {
        _dbContext.Client.UpdateNav(Transform(group))
            .Include(g => g.Roles)
            .ExecuteCommand();
    }

    public async Task BulkUpdate(List<Group> groups)
    {
        var grps = groups.Select(g => Transform(g)).ToList();
        await _dbContext.Client.Fastest<SugarGroup>().BulkUpdateAsync(grps);
    }

    public async Task BulkUpdate(List<GroupRealm> groupRealms)
    {
        var grps = groupRealms.Select(g => Transform(g)).ToList();
        await _dbContext.Client.Fastest<SugarGroupRealm>().BulkUpdateAsync(grps);
    }

    public void DeleteRange(IEnumerable<Group> groups)
    {
        _dbContext.Client.Deleteable(groups.Select(g => Transform(g)).ToList())
            .ExecuteCommand();
    }

    public async Task<Group> Get(string realm, string id, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarGroup>()
                    .Includes(c => c.Realms)
                    .Includes(c => c.Children)
                    .Includes(c => c.Roles)
                    .FirstAsync(g => g.Realms.Any(r => r.RealmsName == realm) && g.Id == id, cancellationToken);
        return result?.ToDomain();
    }

    public async Task<List<Group>> GetAllByFullPath(string realm, string fullPath, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarGroup>()
                .Includes(c => c.Realms)
                .Where(g => g.Realms.Any(r => r.RealmsName == realm) && g.FullPath.StartsWith(fullPath))
                .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<Group>> GetAllByFullPath(string realm, string id, string fullPath, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarGroup>()
                .Includes(c => c.Realms)
                .Where(g => g.Realms.Any(r => r.RealmsName == realm) && g.FullPath.StartsWith(fullPath) && g.Id != id)
                .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<Group>> GetAllByStrictFullPath(List<string> fullPathLst, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarGroup>()
            .Includes(g => g.Roles, r => r.Realms)
            .Includes(c => c.Realms)
            .Where(g => fullPathLst.Contains(g.FullPath))
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<Group>> GetAllByStrictFullPath(string realm, List<string> fullPathLst, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarGroup>()
            .Includes(g => g.Roles, r => r.Realms)
            .Includes(c => c.Realms)
            .Where(g => fullPathLst.Contains(g.FullPath) && g.Realms.Any(r => r.RealmsName == realm))
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public async Task<Group> GetByStrictFullPath(string realm, string fullPath, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarGroup>()
            .Includes(c => c.Realms)
            .Includes(c => c.Children)
            .FirstAsync(g => g.FullPath == fullPath && g.Realms.Any(r => r.RealmsName == realm), CancellationToken.None);
        return result?.ToDomain();
    }

    public async Task<SearchResult<Group>> Search(string realm, SearchGroupsRequest request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Client.Queryable<SugarGroup>()
            .Includes(c => c.Realms)
            .Where(c => c.Realms.Any(r => r.RealmsName == realm) && (request.OnlyRoot  == false || request.OnlyRoot == true && c.Name == c.FullPath));
        /*
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        else
            query = query.OrderBy(q => q.FullPath);
        */
        query = query.OrderByDescending(r => r.UpdateDateTime);
        var nb = query.Count();
        var groups = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
        return new SearchResult<Group>
        {
            Content = groups.Select(g => g.ToDomain()).ToList(),
            Count = nb
        };
    }

    private static SugarGroup Transform(Group group)
    {
        return new SugarGroup
        {
            Id = group.Id,
            FullPath = group.FullPath,
            CreateDateTime = group.CreateDateTime,
            Description = group.Description,
            Name = group.Name,
            ParentGroupId = group.ParentGroupId,
            UpdateDateTime = group.UpdateDateTime,
            Realms = group.Realms.Select(r => Transform(r)).ToList(),
            Users = group.Users.Select(u => new SugarGroupUser
            {
                GroupsId = u.UsersId
            }).ToList(),
            Children = group.Children.Select(u => new SugarGroup
            {
                Id = u.Id
            }).ToList(),
            Roles = group.Roles.Select(r => new SugarScope
            {
                ScopesId = r.Id
            }).ToList()
        };
    }

    private static SugarGroupRealm Transform(GroupRealm realm)
    {
        return new SugarGroupRealm
        {
            GroupsId = realm.GroupsId,
            RealmsName = realm.RealmsName,
        };
    }

    public Task BulkAdd(List<Group> groups)
    {
        throw new NotImplementedException();
    }

    public Task<List<Group>> GetByIds(List<string> ids, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public Task<List<Group>> GetByNames(List<string> names, CancellationToken cancellationToken)
        => throw new NotImplementedException();
}
