// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Uma.Domains;
using SimpleIdServer.Uma.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.EF.Persistence
{
    public class UMAResourceRepository : IUMAResourceRepository
    {
        private readonly UMAEFDbContext _dbContext;

        public UMAResourceRepository(UMAEFDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> Add(UMAResource data, CancellationToken token)
        {
            _dbContext.Resources.Add(data);
            return Task.FromResult(true);
        }

        public Task<bool> Delete(UMAResource data, CancellationToken token)
        {
            _dbContext.Resources.Remove(data);
            return Task.FromResult(true);
        }

        public Task<bool> Update(UMAResource data, CancellationToken token)
        {
            _dbContext.Resources.Update(data);
            return Task.FromResult(true);
        }

        public Task<int> SaveChanges(CancellationToken token)
        {
            return _dbContext.SaveChangesAsync(token);
        }

        public async Task<SearchResult<UMAResource>> Find(SearchUMAResourceParameter searchUMAResourceParameter, CancellationToken cancellationToken)
        {
            IQueryable<UMAResource> resources = GetResources();
            if (!string.IsNullOrWhiteSpace(searchUMAResourceParameter.Subject))
            {
                resources = resources.Where(f => f.Subject == searchUMAResourceParameter.Subject);
            }

            int totalResults = await resources.CountAsync(cancellationToken);
            if (searchUMAResourceParameter.SortKey == "CreateDateTime")
            {
                switch (searchUMAResourceParameter.SortOrder)
                {
                    case SearchRequestOrders.ASC:
                        resources = resources.OrderBy(p => p.CreateDateTime);
                        break;
                    case SearchRequestOrders.DESC:
                        resources = resources.OrderByDescending(p => p.CreateDateTime);
                        break;
                }
            }

            var records = await resources.Skip(searchUMAResourceParameter.StartIndex).Take(searchUMAResourceParameter.Count).ToListAsync(cancellationToken);
            return new SearchResult<UMAResource>
            {
                Records = records,
                TotalResults = totalResults
            };
        }

        public Task<UMAResource> FindByIdentifier(string id, CancellationToken cancellationToken)
        {
            return GetResources().FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<UMAResource>> FindByIdentifiers(IEnumerable<string> ids, CancellationToken cancellationToken)
        {
            IEnumerable<UMAResource> result = await GetResources().Where(r => ids.Contains(r.Id)).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<IEnumerable<UMAResource>> GetAll(CancellationToken cancellationToken)
        {
            IEnumerable<UMAResource> result = await GetResources().ToListAsync(cancellationToken);
            return result;
        }

        private IQueryable<UMAResource> GetResources()
        {
            return _dbContext.Resources
                .Include(r => r.Permissions).ThenInclude(p => p.Claims)
                .Include(r => r.Translations).ThenInclude(r => r.Translation);
        }

        public void Dispose()
        {
        }
    }
}
