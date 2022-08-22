// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Parser.Expressions;
using SimpleIdServer.Scim.Persistence.EF.Extensions;
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

        public Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId)
        {
            return _scimDbContext.SCIMRepresentationLst
                .Include(r => r.FlatAttributes)
                .Include(r => r.Schemas).ThenInclude(s => s.Attributes).FirstOrDefaultAsync(r => r.Id == representationId);
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId, string resourceType)
        {
            return _scimDbContext.SCIMRepresentationLst
                .Include(r => r.FlatAttributes).ThenInclude(s => s.SchemaAttribute)
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
                    parameter.SortOrder ?? SearchSCIMRepresentationOrders.Descending,
                    parameter.StartIndex,
                    parameter.Count,
                    parameter.IncludedAttributes,
                    parameter.ExcludedAttributes);
            }

            int total = queryableRepresentations.Count();
            queryableRepresentations = queryableRepresentations.Skip(parameter.StartIndex <= 1 ? 0 : parameter.StartIndex - 1).Take(parameter.Count);
            return queryableRepresentations.BuildResult(_scimDbContext, parameter.IncludedAttributes, parameter.ExcludedAttributes, total);
        }
    }
}
