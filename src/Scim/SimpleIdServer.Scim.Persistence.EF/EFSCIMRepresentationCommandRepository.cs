// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Domains;
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

        public async Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationByIds(IEnumerable<string> representationIds, string resourceType)
        {
            IEnumerable<SCIMRepresentation> result = await _scimDbContext.SCIMRepresentationLst.Include(r => r.FlatAttributes)
                .Where(r => r.ResourceType == resourceType && representationIds.Contains(r.Id))
                .ToListAsync();
            return result;
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

        public Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationsByAttributeFullPath(string fullPath, IEnumerable<string> value, string resourceType)
        {
            throw new System.NotImplementedException();
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
    }
}
