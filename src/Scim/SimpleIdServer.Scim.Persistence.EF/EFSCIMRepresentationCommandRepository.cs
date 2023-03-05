// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class EFSCIMRepresentationCommandRepository : ISCIMRepresentationCommandRepository
    {
        private readonly SCIMDbContext _scimDbContext;

        public EFSCIMRepresentationCommandRepository(SCIMDbContext scimDbContext)
        {
            _scimDbContext = scimDbContext;
        }

        public async Task<SCIMRepresentation> Get(string id, CancellationToken token = default)
        {
            var result = await _scimDbContext.SCIMRepresentationLst
                .Include(r => r.FlatAttributes)
                .Include(r => r.Schemas).ThenInclude(s => s.Attributes).FirstOrDefaultAsync(r => r.Id == id, token);
            return result;
        }

        public async Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationByIds(IEnumerable<string> representationIds)
        {
            IEnumerable<SCIMRepresentation> result = await _scimDbContext.SCIMRepresentationLst.Include(r => r.FlatAttributes)
                .Where(r => representationIds.Contains(r.Id))
                .ToListAsync();
            return result;
        }

        public IEnumerable<IEnumerable<SCIMRepresentation>> FindPaginatedRepresentations(IEnumerable<string> representationIds,  string resourceType = null, int nbRecords = 50, bool ignoreAttributes = false)
        {
            var nb = representationIds.Count();
            var nbPages = Math.Ceiling((decimal)(nb / nbRecords));
            for(var i = 0; i <= nbPages; i++)
            {
                var filter = representationIds.Skip(i * nbRecords).Take(nbRecords);
                var result = _scimDbContext.SCIMRepresentationLst.Include(r => r.FlatAttributes)
                    .Where(r => filter.Contains(r.Id));
                if (!string.IsNullOrWhiteSpace(resourceType))
                    yield return result.Where(r => r.ResourceType == resourceType);
                else yield return result;
            }
        }

        public IEnumerable<IEnumerable<SCIMRepresentationAttribute>> FindPaginatedGraphAttributes(string valueStr, string schemaAttributeId, int nbRecords = 50, string sourceRepresentationId = null)
        {
            var query = _scimDbContext.SCIMRepresentationAttributeLst.AsNoTracking()
                .Where(a => a.SchemaAttributeId == schemaAttributeId && a.ValueString == valueStr || (sourceRepresentationId != null && a.ValueString == sourceRepresentationId))
                .OrderBy(r => r.ParentAttributeId)
                .Select(r => r.ParentAttributeId);
            var nb = query.Count();
            var nbPages = Math.Ceiling((decimal)(nb / nbRecords));
            for (var i = 0; i <= nbPages; i++)
            {
                var parentIds = query.Skip(i * nbRecords).Take(nbRecords);
                var result = _scimDbContext.SCIMRepresentationAttributeLst.AsNoTracking()
                    .Where(a => parentIds.Contains(a.Id) || parentIds.Contains(a.ParentAttributeId));
                yield return result;
            }
        }

        public IEnumerable<IEnumerable<SCIMRepresentationAttribute>> FindPaginatedGraphAttributes(IEnumerable<string> representationIds,  string valueStr, string schemaAttributeId, int nbRecords = 50, string sourceRepresentationId = null)
        {
            var nb = representationIds.Count();
            var nbPages = Math.Ceiling((decimal)(nb / nbRecords));
            for (var i = 0; i <= nbPages; i++)
            {
                var filter = representationIds.Skip(i * nbRecords).Take(nbRecords);
                var parentIds = _scimDbContext.SCIMRepresentationAttributeLst.AsNoTracking()
                    .Where(a => a.SchemaAttributeId == schemaAttributeId && filter.Contains(a.RepresentationId) && a.ValueString == valueStr || (sourceRepresentationId != null && a.ValueString == sourceRepresentationId))
                    .Select(r => r.ParentAttributeId)
                    .ToList();
                var result = _scimDbContext.SCIMRepresentationAttributeLst.AsNoTracking()
                    .Where(a => parentIds.Contains(a.Id) || parentIds.Contains(a.ParentAttributeId))
                    .ToList();
                yield return result;
            }
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string schemaAttributeId, string value, string endpoint = null)
        {
            return _scimDbContext.SCIMRepresentationAttributeLst
                .Include(a => a.Representation).ThenInclude(a => a.FlatAttributes)
                .Where(a => (endpoint == null || endpoint == a.Representation.ResourceType) && a.SchemaAttributeId == schemaAttributeId && a.ValueString == value)
                .Select(a => a.Representation)
                .FirstOrDefaultAsync();
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string schemaAttributeId, int value, string endpoint = null)
        {
            return _scimDbContext.SCIMRepresentationAttributeLst
                .Include(a => a.Representation).ThenInclude(a => a.FlatAttributes)
                .Where(a => (endpoint == null || endpoint == a.Representation.ResourceType) && a.SchemaAttributeId == schemaAttributeId && a.ValueInteger != null && a.ValueInteger == value)
                .Select(a => a.Representation)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationsByAttributeFullPath(string fullPath, IEnumerable<string> values, string resourceType)
        {
            IEnumerable<SCIMRepresentation> result = await _scimDbContext.SCIMRepresentationLst.Include(r => r.FlatAttributes)
                .Where(r => r.ResourceType == resourceType && r.FlatAttributes.Any(a => a.FullPath == fullPath && values.Contains(a.ValueString)))
                .ToListAsync();
            return result;
        }

        public async Task<ITransaction> StartTransaction(CancellationToken token)
        {
            var transaction = await _scimDbContext.Database.BeginTransactionAsync(token);
            return new EFTransaction(_scimDbContext, transaction);
        }

        public Task<bool> Add(SCIMRepresentation data, CancellationToken token)
        {
            _scimDbContext.SCIMRepresentationLst.Add(data);
            return Task.FromResult(true);
        }

        public Task<bool> Delete(SCIMRepresentation data, CancellationToken token)
        {
            _scimDbContext.SCIMRepresentationLst.Remove(data);
            return Task.FromResult(true);
        }

        public Task<bool> Update(SCIMRepresentation data, CancellationToken token)
        {
            _scimDbContext.SCIMRepresentationLst.Update(data);
            return Task.FromResult(true);
        }

        public Task BulkInsert(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes) => _scimDbContext.BulkInsertAsync(scimRepresentationAttributes.ToList());

        public Task BulkDelete(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes) => _scimDbContext.BulkDeleteAsync(scimRepresentationAttributes.ToList());

        public async Task BulkUpdate(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes)
        {
            var bulkConfig = new BulkConfig
            {
                PropertiesToInclude = new List<string> { nameof(SCIMRepresentationAttribute.ValueString) }
            };
            await _scimDbContext.BulkUpdateAsync(scimRepresentationAttributes.ToList(), bulkConfig);
        }
    }
}
