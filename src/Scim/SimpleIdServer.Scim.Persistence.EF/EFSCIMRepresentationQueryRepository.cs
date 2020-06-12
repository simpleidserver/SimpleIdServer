// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Persistence.EF.Extensions;
using SimpleIdServer.Scim.Persistence.EF.Models;
using System.Collections.Generic;
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

        public async Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string schemaAttributeId, string value, string endpoint = null)
        {
            var record = await IncludeRepresentationNavigationProperties(IncludeRepresentationAttributeNavigationProperties(_scimDbContext.SCIMRepresentationAttributeLst)
                .Where(a => (endpoint == null || endpoint == a.Representation.ResourceType) && a.SchemaAttributeId == schemaAttributeId && a.Values.Any(v => v.ValueString != null && v.ValueString == value))
                .Select(a => a.Representation))
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (record == null)
            {
                return null;
            }

            var result = record.ToDomain();
            _scimDbContext.Entry(record).State = EntityState.Detached;
            return result;
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string schemaAttributeId, int value, string endpoint = null)
        {
            var record = await IncludeRepresentationNavigationProperties(IncludeRepresentationAttributeNavigationProperties(_scimDbContext.SCIMRepresentationAttributeLst)
                .Where(a => (endpoint == null || endpoint == a.Representation.ResourceType) && a.SchemaAttributeId == schemaAttributeId && a.Values.Any(v => v.ValueInteger != null && v.ValueInteger.Value == value))
                .Select(a => a.Representation))
                .AsNoTracking()
                .FirstOrDefaultAsync(); 
            if (record == null)
            {
                return null;
            }

            var result = record.ToDomain();
            _scimDbContext.Entry(record).State = EntityState.Detached;
            return result;
        }

        public async Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationByAttributes(string schemaAttributeId, IEnumerable<string> values, string endpoint = null)
        {
            var records = await IncludeRepresentationNavigationProperties(IncludeRepresentationAttributeNavigationProperties(_scimDbContext.SCIMRepresentationAttributeLst)
                .Where(a => (endpoint == null || endpoint == a.Representation.ResourceType) && a.SchemaAttributeId == schemaAttributeId && a.Values.Any(v => values.Contains(v.ValueString)))
                .Select(a => a.Representation))
                .AsNoTracking()
                .ToListAsync();
            return records.Select(r => r.ToDomain());
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId)
        {
            var record = await IncludeRepresentationNavigationProperties(_scimDbContext.SCIMRepresentationLst).FirstOrDefaultAsync(r => r.Id == representationId);
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
            var record = await IncludeRepresentationNavigationProperties(_scimDbContext.SCIMRepresentationLst).FirstOrDefaultAsync(r => r.Id == representationId && r.ResourceType == resourceType);
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
            IQueryable<SCIMRepresentationModel> queryableRepresentations = IncludeRepresentationNavigationProperties(_scimDbContext.SCIMRepresentationLst)
                .Where(s => s.ResourceType == parameter.ResourceType);
            if (parameter.Filter != null)
            {
                var evaluatedExpression = parameter.Filter.Evaluate(queryableRepresentations);
                queryableRepresentations = (IQueryable<SCIMRepresentationModel>)evaluatedExpression.Compile().DynamicInvoke(queryableRepresentations);
            }

            int totalResults = queryableRepresentations.Count();
            IEnumerable<SCIMRepresentation> result = new List<SCIMRepresentation>();
            if (parameter.Count > 0)
            {
                result = queryableRepresentations.Skip(parameter.StartIndex).Take(parameter.Count).ToList().Select(s => s.ToDomain());
            }

            return Task.FromResult(new SearchSCIMRepresentationsResponse(totalResults, result));
        }

        private static IQueryable<SCIMRepresentationAttributeModel> IncludeRepresentationAttributeNavigationProperties(IQueryable<SCIMRepresentationAttributeModel> attributes)
        {
            return attributes.Include(a => a.Values)
                .Include(a => a.Representation);
        }

        private static IQueryable<SCIMRepresentationModel> IncludeRepresentationNavigationProperties(IQueryable<SCIMRepresentationModel> representations)
        {
            return representations.Include(s => s.Attributes).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Attributes).ThenInclude(s => s.Values)
                .Include(s => s.Attributes).ThenInclude(s => s.Children).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Attributes).ThenInclude(s => s.Children).ThenInclude(s => s.Values)
                .Include(s => s.Attributes).ThenInclude(s => s.Children).ThenInclude(s => s.Children).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Attributes).ThenInclude(s => s.Children).ThenInclude(s => s.Children).ThenInclude(s => s.Values)
                .Include(s => s.Schemas).ThenInclude(s => s.Schema).ThenInclude(s => s.Attributes)
                .Include(s => s.Schemas).ThenInclude(s => s.Schema).ThenInclude(s => s.SchemaExtensions);
        }
    }
}
