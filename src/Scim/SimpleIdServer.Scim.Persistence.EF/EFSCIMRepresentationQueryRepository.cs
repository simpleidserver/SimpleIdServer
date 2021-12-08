// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Persistence.EF.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId)
        {
            return _scimDbContext.SCIMRepresentationLst.
                Include(r => r.FlatAttributes)
                .Include(r => r.Schemas).ThenInclude(s => s.Attributes).FirstOrDefaultAsync(r => r.Id == representationId);
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId, string resourceType)
        {
            return _scimDbContext.SCIMRepresentationLst
                .Include(r => r.FlatAttributes)
                .Include(r => r.Schemas).ThenInclude(s => s.Attributes)
                .FirstOrDefaultAsync(r => r.Id == representationId && r.ResourceType == resourceType);
        }

        public async Task<SearchSCIMRepresentationsResponse> FindSCIMRepresentations(SearchSCIMRepresentationsParameter parameter)
        {
            IQueryable<SCIMRepresentation> queryableRepresentations = _scimDbContext.SCIMRepresentationLst
                .Include(r => r.FlatAttributes)
                .Where(s => s.ResourceType == parameter.ResourceType);
            if (parameter.SortBy == null)
            {
                queryableRepresentations = queryableRepresentations.OrderBy(s => s.Id);
            }

            if (parameter.Filter != null)
            {
                var evaluatedExpression = parameter.Filter.Evaluate(queryableRepresentations);
                queryableRepresentations = (IQueryable<SCIMRepresentation>)evaluatedExpression.Compile().DynamicInvoke(queryableRepresentations);
            }

            if(parameter.SortBy != null)
            {
                return await parameter.SortBy.EvaluateOrderBy(
                    _scimDbContext,
                    queryableRepresentations,
                    parameter.SortOrder.Value,
                    parameter.StartIndex,
                    parameter.Count,
                    parameter.IncludedAttributes,
                    parameter.ExcludedAttributes);
            }

            int total = queryableRepresentations.Count();
            queryableRepresentations = queryableRepresentations.Skip(parameter.StartIndex <= 1 ? 0 : parameter.StartIndex - 1).Take(parameter.Count);
            return queryableRepresentations.BuildResult(_scimDbContext, parameter.IncludedAttributes, parameter.ExcludedAttributes, total);
        }

        public async Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationByIds(IEnumerable<string> representationIds, string resourceType)
        {
            IEnumerable<SCIMRepresentation> result = await _scimDbContext.SCIMRepresentationLst.Include(r => r.FlatAttributes).Include(r => r.Schemas).ThenInclude(s => s.Attributes)
                .Where(r => r.ResourceType == resourceType && representationIds.Contains(r.Id))
                .ToListAsync();
            return result;
        }
    }
}
