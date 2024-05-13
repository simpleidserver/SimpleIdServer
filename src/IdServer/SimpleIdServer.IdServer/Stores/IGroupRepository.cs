// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api.Groups;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IGroupRepository
{
    Task<SearchResult<Group>> Search(string realm, SearchGroupsRequest request, CancellationToken cancellationToken);
    Task<Group> Get(string realm, string id, CancellationToken cancellationToken);
    Task<Group> GetByFullPath(string realm, string fullPath, CancellationToken cancellationToken);
    Task<List<Group>> GetAllByFullPath(string realm, string fullPath, CancellationToken cancellationToken);
    Task<List<Group>> GetAllByFullPath(string realm, string id, string fullPath, CancellationToken cancellationToken);
    IQueryable<Group> Query();
    void Add(Group group);
    void DeleteRange(IEnumerable<Group> groups);
    Task<int> SaveChanges(CancellationToken cancellationToken);
    Task BulkUpdate(List<Group> groups);
    Task BulkUpdate(List<GroupRealm> groupRealms);
}
