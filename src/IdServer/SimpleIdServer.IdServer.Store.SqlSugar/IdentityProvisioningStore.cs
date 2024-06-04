// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar
{
    public class IdentityProvisioningStore : IIdentityProvisioningStore
    {
        private readonly DbContext _dbContext;

        public IdentityProvisioningStore(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void DeleteRange(IEnumerable<IdentityProvisioning> identityProvisioningLst)
        {
            var lst = identityProvisioningLst.Select(i => SugarIdentityProvisioning.Transform(i)).ToList();
            _dbContext.Client.Deleteable(lst).ExecuteCommand();
        }

        public async Task<IdentityProvisioning> Get(string realm, string id, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarIdentityProvisioning>()
                    .Includes(p => p.Realms)
                    .Includes(p => p.Histories)
                    .Includes(p => p.Definition, d => d.MappingRules)
                    .FirstAsync(p => p.Realms.Any(r => r.RealmsName == realm) && p.Id == id, cancellationToken);
            return result?.ToDomain();
        }

        public void Remove(IdentityProvisioning identityProvisioning)
        {
            _dbContext.Client.Deleteable(SugarIdentityProvisioning.Transform(identityProvisioning)).ExecuteCommand();
        }

        public void Update(IdentityProvisioning identityProvisioning)
        {
            var transformedIdentityProvisioning = SugarIdentityProvisioning.Transform(identityProvisioning);
            _dbContext.Client.Updateable(transformedIdentityProvisioning).ExecuteCommand();
            _dbContext.Client.UpdateNav(SugarIdentityProvisioning.Transform(identityProvisioning))
                .Include(c => c.Histories)
                .Include(c => c.Realms)
                .ExecuteCommand();
        }

        public void Add(IdentityProvisioningDefinition identityProvisioningDefinition)
        {
            _dbContext.Client.InsertNav(SugarIdentityProvisioningDefinition.Transform(identityProvisioningDefinition))
                .Include(c => c.MappingRules)
                .ExecuteCommand();
        }

        public void Update(IdentityProvisioningDefinition identityProvisioningDefinition)
        {
            _dbContext.Client.UpdateNav(SugarIdentityProvisioningDefinition.Transform(identityProvisioningDefinition))
                .Include(c => c.MappingRules)
                .ExecuteCommand();
        }

        public async Task<SearchResult<IdentityProvisioning>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Client.Queryable<SugarIdentityProvisioning>()
                .Includes(p => p.Realms)
                .Where(p => p.Realms.Any(r => r.RealmsName == realm));
            /*
            if (!string.IsNullOrWhiteSpace(request.Filter))
                query = query.Where(request.Filter);

            if (!string.IsNullOrWhiteSpace(request.OrderBy))
                query = query.OrderBy(request.OrderBy);
            */
            query = query.OrderByDescending(c => c.UpdateDateTime);
            var nb = query.Count();
            var idProviders = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
            return new SearchResult<IdentityProvisioning>
            {
                Count = nb,
                Content = idProviders.Select(i => i.ToDomain()).ToList()
            };
        }
    }
}
