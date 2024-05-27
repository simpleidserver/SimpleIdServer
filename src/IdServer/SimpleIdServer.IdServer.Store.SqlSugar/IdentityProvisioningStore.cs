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
            var lst = identityProvisioningLst.Select(i => Transform(i)).ToList();
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
            _dbContext.Client.Deleteable(Transform(identityProvisioning)).ExecuteCommand();
        }

        public void Update(IdentityProvisioning identityProvisioning)
        {
            _dbContext.Client.UpdateNav(Transform(identityProvisioning))
                .Include(c => c.Histories)
                .Include(c => c.Definition)
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

        private static SugarIdentityProvisioning Transform(IdentityProvisioning identityProvisioning)
        {
            return new SugarIdentityProvisioning
            {
                Id = identityProvisioning.Id,
                Description = identityProvisioning.Description,
                Name = identityProvisioning.Name,
                Histories = identityProvisioning.Histories.Select(h => Transform(h)).ToList(),
                Definition = Transform(identityProvisioning.Definition)
            };
        }

        private static SugarIdentityProvisioningHistory Transform(IdentityProvisioningHistory history)
        {
            return new SugarIdentityProvisioningHistory
            {
                CurrentPage = history.CurrentPage,
                ExecutionDateTime = history.ExecutionDateTime,
                NbFilteredRepresentations = history.NbFilteredRepresentations,
                NbGroups = history.NbGroups,
                NbUsers = history.NbUsers,
                ProcessId = history.ProcessId,
                Status = history.Status,
                TotalPages = history.TotalPages,
            };
        }

        private static SugarIdentityProvisioningDefinition Transform(IdentityProvisioningDefinition definition)
        {
            return new SugarIdentityProvisioningDefinition
            {
                CreateDateTime = definition.CreateDateTime,
                Description = definition.Description,
                Name = definition.Name,
                OptionsFullQualifiedName = definition.OptionsFullQualifiedName,
                OptionsName = definition.OptionsName,
                UpdateDateTime = definition.UpdateDateTime,
                MappingRules = definition.MappingRules.Select(m => Transform(m)).ToList()
            };
        }

        private static SugarIdentityProvisioningMappingRule Transform(IdentityProvisioningMappingRule rule)
        {
            return new SugarIdentityProvisioningMappingRule
            {
                From = rule.From,
                HasMultipleAttribute = rule.HasMultipleAttribute,
                Id = rule.Id,
                MapperType = rule.MapperType,
                TargetUserAttribute = rule.TargetUserAttribute,
                TargetUserProperty = rule.TargetUserProperty,
                Usage = rule.Usage
            };
        }
    }
}
