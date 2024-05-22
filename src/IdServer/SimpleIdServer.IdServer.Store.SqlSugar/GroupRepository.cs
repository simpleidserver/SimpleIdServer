// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Api.Groups;
using SimpleIdServer.IdServer.Domains;
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

    public Task BulkUpdate(List<Group> groups)
    {
        throw new NotImplementedException();
    }

    public Task BulkUpdate(List<GroupRealm> groupRealms)
    {
        throw new NotImplementedException();
    }

    public void DeleteRange(IEnumerable<Group> groups)
    {
        throw new NotImplementedException();
    }

    public Task<Group> Get(string realm, string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<Group>> GetAllByFullPath(string realm, string fullPath, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<Group>> GetAllByFullPath(string realm, string id, string fullPath, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<Group>> GetAllByStrictFullPath(string realm, List<string> fullPathLst, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Group> GetByStrictFullPath(string realm, string fullPath, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IQueryable<Group> Query()
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<SearchResult<Group>> Search(string realm, SearchGroupsRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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
            Realms = group.Realms.Select(r => new SugarGroupRealm
            {
                RealmsName = r.RealmsName
            }).ToList(),
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
}
