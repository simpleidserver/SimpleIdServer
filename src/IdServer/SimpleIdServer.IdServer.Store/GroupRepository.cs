// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IGroupRepository
    {
        IQueryable<Group> Query();
        void Add(Group group);
        void DeleteRange(IEnumerable<Group> groups);
        Task<int> SaveChanges(CancellationToken cancellationToken);
        Task BulkUpdate(List<Group> groups);
        Task BulkUpdate(List<GroupRealm> groupRealms);
    }

    public class GroupRepository : IGroupRepository
    {
        private readonly StoreDbContext _dbContext;

        public GroupRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Group> Query() => _dbContext.Groups;

        public void Add(Group group) => _dbContext.Groups.Add(group);

        public void DeleteRange(IEnumerable<Group> groups) => _dbContext.Groups.RemoveRange(groups);

        public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);


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
            var existingGroups = await _dbContext.Groups
                .Include(u => u.Realms)
                .Where(u => groupIds.Contains(u.Id)).ToListAsync();
            foreach (var existingGroup in existingGroups)
            {
                existingGroup.Realms = groupRealms.Where(r => r.GroupsId == existingGroup.Id).ToList();
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
