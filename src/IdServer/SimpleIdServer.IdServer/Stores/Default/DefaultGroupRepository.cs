// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Org.BouncyCastle.Crypto;
using SimpleIdServer.IdServer.Api.Groups;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultGroupRepository : IGroupRepository
{
    private readonly List<Group> _groups;

    public DefaultGroupRepository(List<Group> groups)
    {
        _groups = groups;
    }

    public async Task<SearchResult<Group>> Search(string realm, SearchGroupsRequest request, CancellationToken cancellationToken)
    {
        var query = _groups.AsQueryable().Where(g => g.Realms.Any(r => r.RealmsName == realm) && (!request.OnlyRoot || g.Name == g.FullPath));
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        else
            query = query.OrderBy(g => g.FullPath);
        var nb = query.Count();
        var groups = query.Skip(request.Skip ?? 0).Take(request.Take ?? nb).ToList();
        return await Task.FromResult(new SearchResult<Group>
        {
            Content = groups,
            Count = nb
        });
    }

    public Task<Group> Get(string realm, string id, CancellationToken cancellationToken)
    {
        var group = _groups.FirstOrDefault(g => g.Realms.Any(r => r.RealmsName == realm) && g.Id == id);
        return Task.FromResult(group);
    }

    public Task<Group> GetByStrictFullPath(string realm, string fullPath, CancellationToken cancellationToken)
    {
        var group = _groups.FirstOrDefault(g => g.FullPath == fullPath && g.Realms.Any(r => r.RealmsName == realm));
        return Task.FromResult(group);
    }

    public Task<List<Group>> GetAllByFullPath(string realm, string fullPath, CancellationToken cancellationToken)
    {
        var list = _groups.Where(g => g.Realms.Any(r => r.RealmsName == realm) && g.FullPath.StartsWith(fullPath)).ToList();
        return Task.FromResult(list);
    }

    public Task<List<Group>> GetAllByStrictFullPath(List<string> fullPathLst, CancellationToken cancellationToken)
    {
        var list = _groups.Where(g => fullPathLst.Contains(g.FullPath)).ToList();
        return Task.FromResult(list);
    }

    public Task<List<Group>> GetAllByStrictFullPath(string realm, List<string> fullPathLst, CancellationToken cancellationToken)
    {
        var list = _groups.Where(g => fullPathLst.Contains(g.FullPath) && g.Realms.Any(r => r.RealmsName == realm)).ToList();
        return Task.FromResult(list);
    }

    public Task<List<Group>> GetAllByFullPath(string realm, string id, string fullPath, CancellationToken cancellationToken)
    {
        var list = _groups.Where(g => g.Realms.Any(r => r.RealmsName == realm) && g.FullPath.StartsWith(fullPath) && g.Id != id).ToList();
        return Task.FromResult(list);
    }

    public void Add(Group group) => _groups.Add(group);

    public void Update(Group group)
    {
        var existing = _groups.FirstOrDefault(g => g.Id == group.Id);
        if (existing != null)
        {
            _groups.Remove(existing);
            _groups.Add(group);
        }
    }

    public void DeleteRange(IEnumerable<Group> groups)
    {
        foreach (var group in groups)
        {
            _groups.Remove(group);
        }
    }

    public async Task BulkUpdate(List<Group> groups)
    {
        var groupIds = groups.Select(g => g.Id).ToList();
        _groups.RemoveAll(g => groupIds.Contains(g.Id));
        _groups.AddRange(groups);
        await Task.CompletedTask;
    }

    public async Task BulkUpdate(List<GroupRealm> groupRealms)
    {
        foreach (var gr in groupRealms)
        {
            var group = _groups.FirstOrDefault(g => g.Id == gr.GroupsId);
            if (group != null && !group.Realms.Any(r => r.RealmsName == gr.RealmsName))
                group.Realms.Add(new GroupRealm { RealmsName = gr.RealmsName });
        }
        await Task.CompletedTask;
    }

    public Task BulkAdd(List<Group> groups)
    {
        _groups.AddRange(groups);
        return Task.CompletedTask;
    }

    public Task<List<Group>> GetByIds(List<string> ids, CancellationToken cancellationToken)
    {
        var list = _groups.Where(g => ids.Contains(g.Id)).ToList();
        return Task.FromResult(list);
    }
    
    public Task<List<Group>> GetByNames(List<string> names, CancellationToken cancellationToken)
    {
        var list = _groups.Where(g => names.Contains(g.Name)).ToList();
        return Task.FromResult(list);
    }
}
