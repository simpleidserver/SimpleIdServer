// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store
{
    public interface IGroupRepository
    {
        IQueryable<Group> Query();
        void Add(Group group);
        void DeleteRange(IEnumerable<Group> groups);
        Task<int> SaveChanges(CancellationToken cancellationToken);
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
    }
}
