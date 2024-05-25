// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Api.Groups;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Store.EF;

public class GroupRepository : IGroupRepository
{
    private readonly StoreDbContext _dbContext;

    public GroupRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SearchResult<Group>> Search(string realm, SearchGroupsRequest request, CancellationToken cancellationToken)
    {
        IQueryable<Group> query = _dbContext.Groups
            .Include(c => c.Realms)
            .Where(c => c.Realms.Any(r => r.RealmsName == realm) && (!request.OnlyRoot || request.OnlyRoot && c.Name == c.FullPath))
            .AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        else
            query = query.OrderBy(q => q.FullPath);

        var nb = query.Count();
        var groups = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
        return new SearchResult<Group>
        {
            Content = groups,
            Count = nb
        };
    }

    public Task<Group> Get(string realm, string id, CancellationToken cancellationToken)
        => _dbContext.Groups
                    .Include(c => c.Realms)
                    .Include(c => c.Children)
                    .Include(c => c.Roles)
                    .SingleOrDefaultAsync(g => g.Realms.Any(r => r.RealmsName == realm) && g.Id == id, cancellationToken);

    public Task<Group> GetByStrictFullPath(string realm, string fullPath, CancellationToken cancellationToken)
        => _dbContext.Groups
            .Include(c => c.Realms)
            .Include(c => c.Children)
            .SingleOrDefaultAsync(g => g.FullPath == fullPath && g.Realms.Any(r => r.RealmsName == realm), CancellationToken.None);

    public Task<List<Group>> GetAllByFullPath(string realm, string fullPath, CancellationToken cancellationToken)
        => _dbContext.Groups
                .Include(c => c.Realms)
                .Where(g => g.Realms.Any(r => r.RealmsName == realm) && g.FullPath.StartsWith(fullPath))
                .ToListAsync(cancellationToken);

    public Task<List<Group>> GetAllByStrictFullPath(string realm, List<string> fullPathLst, CancellationToken cancellationToken)
        => _dbContext.Groups
            .Include(g => g.Roles)
            .Include(c => c.Realms)
            .Where(g => fullPathLst.Contains(g.FullPath) && g.Realms.Any(r => r.RealmsName == realm))
            .ToListAsync(cancellationToken);


    public Task<List<Group>> GetAllByFullPath(string realm, string id, string fullPath, CancellationToken cancellationToken)
        => _dbContext.Groups
                .Include(c => c.Realms)
                .Where(g => g.Realms.Any(r => r.RealmsName == realm) && g.FullPath.StartsWith(fullPath) && g.Id != id)
                .ToListAsync(cancellationToken);

    public void Add(Group group) => _dbContext.Groups.Add(group);

    public void Update(Group group)
    {

    }

    public void DeleteRange(IEnumerable<Group> groups) => _dbContext.Groups.RemoveRange(groups);

    public virtual async Task BulkUpdate(List<Group> groups)
    {
        if (_dbContext.Database.IsRelational())
        {
            var bulkConfig = new BulkConfig
            {
                PropertiesToIncludeOnCompare = new List<string> { nameof(Group.Id) }
            };
            await _dbContext.BulkInsertOrUpdateAsync(groups, bulkConfig);
            return;
        }

        var groupIds = groups.Select(u => u.Id).ToList();
        var existingGroups = await _dbContext.Groups.Where(u => groupIds.Contains(u.Id)).ToListAsync();
        _dbContext.Groups.RemoveRange(existingGroups);
        _dbContext.Groups.AddRange(groups);
        await _dbContext.SaveChangesAsync();
    }

    public async Task BulkUpdate(List<GroupRealm> groupRealms)
    {
        if (_dbContext.Database.IsRelational())
        {
            var bulkConfig = new BulkConfig
            {
                PropertiesToIncludeOnCompare = new List<string> { nameof(GroupRealm.GroupsId), nameof(GroupRealm.RealmsName) }
            };
            await _dbContext.BulkInsertOrUpdateAsync(groupRealms, bulkConfig);
        }

        var groupIds = groupRealms.Select(r => r.GroupsId).ToList();
        var existingRealms = await _dbContext.GroupRealm
            .Where(u => groupIds.Contains(u.GroupsId))
            .ToListAsync();
        var newRealms = groupRealms.Where(g => !existingRealms.Any(r => r.GroupsId == g.GroupsId && r.RealmsName == g.RealmsName));
        _dbContext.GroupRealm.AddRange(newRealms);
        await _dbContext.SaveChangesAsync();
    }
}
