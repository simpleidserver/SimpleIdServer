// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Parser.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class EFSCIMRepresentationCommandRepository : ISCIMRepresentationCommandRepository
    {
        private readonly SCIMDbContext _scimDbContext;
        private readonly SCIMEFOptions _options;

        public EFSCIMRepresentationCommandRepository(SCIMDbContext scimDbContext, IOptions<SCIMEFOptions> options)
        {
            _scimDbContext = scimDbContext;
            _options = options.Value;
        }

        public async Task<SCIMRepresentation> Get(string id, CancellationToken token = default)
        {
            var result = await _scimDbContext.SCIMRepresentationLst
                .Include(r => r.FlatAttributes)
                .Include(r => r.Schemas).ThenInclude(s => s.Attributes).FirstOrDefaultAsync(r => r.Id == id, token);
            return result;
        }
        
        public async Task<List<SCIMRepresentation>> FindPaginatedRepresentations(List<string> representationIds, string resourceType = null, int nbRecords = 50, bool ignoreAttributes = false)
        {
            var chunks = representationIds.Chunk(nbRecords).ToList();
            var representations = new List<SCIMRepresentation>();
            foreach (var chunk in chunks) {
                var query = _scimDbContext.SCIMRepresentationLst
                    .Include(r => r.FlatAttributes)
                    .Where(r => chunk.Contains(r.Id))
                    .AsNoTracking();
                if (!string.IsNullOrWhiteSpace(resourceType))
                    query = query.Where(r => r.ResourceType == resourceType);

                var result = await query.ToListAsync();
                representations.AddRange(result);
            }

            return representations;
        }

        public async Task<List<SCIMRepresentationAttribute>> FindPaginatedGraphAttributes(string valueStr, string schemaAttributeId, int nbRecords = 50, string sourceRepresentationId = null)
        {
            var parentAttributeIds = await GetParentAttributeIds(valueStr, schemaAttributeId, sourceRepresentationId);
            var result = await GetRelatedAttributes(nbRecords, parentAttributeIds);
            return result;
        }

        private async Task<List<string>> GetParentAttributeIds(string valueStr, string schemaAttributeId, string sourceRepresentationId)
        {
            var parentAttributeIds = await _scimDbContext.SCIMRepresentationAttributeLst.AsNoTracking()
                .Where(a => a.SchemaAttributeId == schemaAttributeId && a.ValueString == valueStr || (sourceRepresentationId != null && a.ValueString == sourceRepresentationId))
                .OrderBy(r => r.ParentAttributeId)
                .Select(r => r.ParentAttributeId)
                .AsNoTracking()
                .ToListAsync();
            return parentAttributeIds;
        }

        private async Task<List<SCIMRepresentationAttribute>> GetRelatedAttributes(int nbRecords, List<string> parentAttributeIds)
        {
            var result = new List<SCIMRepresentationAttribute>();
            var chunks = parentAttributeIds.Chunk(nbRecords).ToList();
            foreach (var chunk in chunks) {
                result.AddRange(
                    await _scimDbContext.SCIMRepresentationAttributeLst.AsNoTracking()
                    .Where(a => chunk.Contains(a.Id) || chunk.Contains(a.ParentAttributeId))
                    .ToListAsync()
                );
            }
            return result;
        }

        public async Task<List<SCIMRepresentationAttribute>> FindPaginatedGraphAttributes(IEnumerable<string> representationIds, string valueStr, string schemaAttributeId, int nbRecords = 50, string sourceRepresentationId = null)
        {
            var parentIds = await _scimDbContext.SCIMRepresentationAttributeLst.AsNoTracking()
                .Where(a => a.SchemaAttributeId == schemaAttributeId &&
                            representationIds.Contains(a.RepresentationId) &&
                            a.ValueString == valueStr ||
                            (sourceRepresentationId != null && a.ValueString == sourceRepresentationId))
                .Select(r => r.ParentAttributeId)
                .AsNoTracking()
                .Distinct()
                .ToListAsync();
            var attributes = new List<SCIMRepresentationAttribute>();
            var chunks = parentIds.Chunk(nbRecords).ToList();
            foreach (var chunk in chunks) {
                var result = await _scimDbContext.SCIMRepresentationAttributeLst.AsNoTracking()
                    .Where(a => chunk.Contains(a.Id) || chunk.Contains(a.ParentAttributeId))
                    .AsNoTracking()
                    .ToListAsync();
                attributes.AddRange(result);
            }
            return attributes;
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

        public async Task<List<SCIMRepresentation>> FindSCIMRepresentationsByAttributeFullPath(string fullPath, IEnumerable<string> values, string resourceType)
        {
            List<SCIMRepresentation> result = await _scimDbContext.SCIMRepresentationLst.Include(r => r.FlatAttributes)
                .Where(r => r.ResourceType == resourceType && r.FlatAttributes.Any(a => a.FullPath == fullPath && values.Contains(a.ValueString)))
                .AsNoTracking()
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

        public Task BulkInsert(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes)
        {
            var bulkConfig = GetBulkConfig(BulkOperations.INSERT);
            return _scimDbContext.BulkInsertAsync(scimRepresentationAttributes.ToList(), bulkConfig);
        }

        public Task BulkDelete(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes)
        {
            var bulkConfig = GetBulkConfig(BulkOperations.DELETE);
            return _scimDbContext.BulkDeleteAsync(scimRepresentationAttributes.ToList(), bulkConfig);
        }

        public async Task BulkUpdate(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes)
        {
            var bulkConfig = new BulkConfig
            {
                PropertiesToInclude = new List<string> { nameof(SCIMRepresentationAttribute.ValueString) }
            };
            bulkConfig = GetBulkConfig(BulkOperations.UPDATE, bulkConfig);
            await _scimDbContext.BulkUpdateAsync(scimRepresentationAttributes.ToList(), bulkConfig);
        }

        private BulkConfig GetBulkConfig(BulkOperations operation) => GetBulkConfig(operation, null);

        private BulkConfig GetBulkConfig(BulkOperations operation, BulkConfig bulkConfig)
        {
            if (!_options.BulkOperations.ContainsKey(operation)) return bulkConfig;
            if (bulkConfig == null) bulkConfig = new BulkConfig();
            _options.BulkOperations[operation](bulkConfig);
            return bulkConfig;
        }

        public Task<List<SCIMRepresentationAttribute>> FindAttributes(string representationId, SCIMAttributeExpression attrExpression, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
