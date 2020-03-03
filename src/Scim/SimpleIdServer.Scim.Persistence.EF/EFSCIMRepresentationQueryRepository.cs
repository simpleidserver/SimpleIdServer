// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Persistence.EF.Extensions;
using SimpleIdServer.Scim.Persistence.EF.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class EFSCIMRepresentationQueryRepository : ISCIMRepresentationQueryRepository
    {
        private readonly SCIMDbContext _scimDbContext;

        public EFSCIMRepresentationQueryRepository(SCIMDbContext scimDbContext)
        {
            _scimDbContext = scimDbContext;
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string attributeId, string value, string endpoint = null)
        {
            var record = await _scimDbContext.SCIMRepresentationLst
                .Include(s => s.Attributes).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Attributes).ThenInclude(s => s.Values).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Attributes).ThenInclude(s => s.Values).ThenInclude(s => s.Values).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Schemas).ThenInclude(s => s.Schema).ThenInclude(s => s.Attributes)
                .Include(s => s.Schemas).ThenInclude(s => s.Schema).ThenInclude(s => s.SchemaExtensions)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => (endpoint == null || endpoint == r.ResourceType) && r.Attributes.Any(a => a.SchemaAttribute.Id == attributeId && a.ValuesString.Contains(value)));
            if (record == null)
            {
                return null;
            }

            var result = record.ToDomain();
            _scimDbContext.Entry(record).State = EntityState.Detached;
            return result;
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string attributeId, int value, string endpoint = null)
        {
            var record = await _scimDbContext.SCIMRepresentationLst
                .Include(s => s.Attributes).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Attributes).ThenInclude(s => s.Values).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Attributes).ThenInclude(s => s.Values).ThenInclude(s => s.Values).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Schemas).ThenInclude(s => s.Schema).ThenInclude(s => s.Attributes)
                .Include(s => s.Schemas).ThenInclude(s => s.Schema).ThenInclude(s => s.SchemaExtensions)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => (endpoint == null || endpoint == r.ResourceType) && r.Attributes.Any(a => a.SchemaAttribute.Id == attributeId && a.ValuesInteger.Contains(value)));
            if (record == null)
            {
                return null;
            }

            var result = record.ToDomain();
            _scimDbContext.Entry(record).State = EntityState.Detached;
            return result;
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId)
        {
            var record = await _scimDbContext.SCIMRepresentationLst
                .Include(s => s.Attributes).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Attributes).ThenInclude(s => s.Values).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Attributes).ThenInclude(s => s.Values).ThenInclude(s => s.Values).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Schemas).ThenInclude(s => s.Schema).ThenInclude(s => s.Attributes)
                .Include(s => s.Schemas).ThenInclude(s => s.Schema).ThenInclude(s => s.SchemaExtensions)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == representationId);
            if (record == null)
            {
                return null;
            }

            var result = record.ToDomain();
            _scimDbContext.Entry(record).State = EntityState.Detached;
            return result;
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId, string resourceType)
        {
            var record = await _scimDbContext.SCIMRepresentationLst
                .Include(s => s.Attributes).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Attributes).ThenInclude(s => s.Values).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Attributes).ThenInclude(s => s.Values).ThenInclude(s => s.Values).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Schemas).ThenInclude(s => s.Schema).ThenInclude(s => s.Attributes)
                .Include(s => s.Schemas).ThenInclude(s => s.Schema).ThenInclude(s => s.SchemaExtensions)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == representationId && r.ResourceType == resourceType);
            if (record == null)
            {
                return null;
            }

            var result = record.ToDomain();
            _scimDbContext.Entry(record).State = EntityState.Detached;
            return result;
        }

        public Task<SearchSCIMRepresentationsResponse> FindSCIMRepresentations(SearchSCIMRepresentationsParameter parameter)
        {
            IQueryable<SCIMRepresentationModel> queryableRepresentations = _scimDbContext.SCIMRepresentationLst
                .Include(s => s.Attributes).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Attributes).ThenInclude(s => s.Values).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Attributes).ThenInclude(s => s.Values).ThenInclude(s => s.Values).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Schemas).ThenInclude(s => s.Schema).ThenInclude(s => s.Attributes)
                .Include(s => s.Schemas).ThenInclude(s => s.Schema).ThenInclude(s => s.SchemaExtensions)
                .AsNoTracking();
            if (parameter.Filter != null)
            {
                var evaluatedExpression = parameter.Filter.Evaluate(queryableRepresentations);
                queryableRepresentations = (IQueryable<SCIMRepresentationModel>)evaluatedExpression.Compile().DynamicInvoke(queryableRepresentations);
            }

            int totalResults = queryableRepresentations.Count();
            var result = queryableRepresentations.Skip(parameter.StartIndex).Take(parameter.Count).ToList().Select(s => s.ToDomain());
            return Task.FromResult(new SearchSCIMRepresentationsResponse(totalResults, result));
        }
    }
}
