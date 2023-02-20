// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Parser.Expressions;
using SimpleIdServer.Scim.Persistence.MongoDB.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.MongoDB
{
    public class SCIMRepresentationQueryRepository : ISCIMRepresentationQueryRepository
    {
        private readonly SCIMDbContext _scimDbContext;

        public SCIMRepresentationQueryRepository(SCIMDbContext scimDbContext)
        {
            _scimDbContext = scimDbContext;
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId)
        {
            var collection = _scimDbContext.SCIMRepresentationLst;
            var result = await collection.AsQueryable().Where(a => a.Id == representationId).ToMongoFirstAsync();
            if (result == null)
            {
                return null;
            }

            result.IncludeAll(_scimDbContext.Database);
            return result;
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId, string resourceType)
        {
            var collection = _scimDbContext.SCIMRepresentationLst;
            var result = await collection.AsQueryable().Where(a => a.Id == representationId && a.ResourceType == resourceType).ToMongoFirstAsync();
            if (result == null)
            {
                return null;
            }

            result.IncludeAll(_scimDbContext.Database);
            return result;
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId, string resourceType, GetSCIMResourceParameter parameter)
        {
            return FindSCIMRepresentationById(representationId, resourceType);
        }

        public Task<SearchSCIMRepresentationsResponse> FindSCIMRepresentations(SearchSCIMRepresentationsParameter parameter)
        {
            IEnumerable<SCIMRepresentation> result = null;
            int totalResults = 0;
            var collection = _scimDbContext.SCIMRepresentationLst;
            var queryableRepresentations = collection.AsQueryable().Where(s => s.ResourceType == parameter.ResourceType);
            if(parameter.SortBy == null)
                queryableRepresentations = queryableRepresentations.OrderBy(s => s.Id);

            if (parameter.Filter != null)
            {
                var evaluatedExpression = parameter.Filter.Evaluate(queryableRepresentations);
                var filtered = evaluatedExpression.Compile().DynamicInvoke(queryableRepresentations) as IMongoQueryable<SCIMRepresentation>;
                totalResults = filtered.Count();
                var representations = filtered.Skip(parameter.StartIndex <= 1 ? 0 : parameter.StartIndex - 1).Take(parameter.Count);
                result = representations.ToList();
            }
            else
            {
                totalResults = queryableRepresentations.Count();
                var representations = queryableRepresentations.Skip(parameter.StartIndex <= 1 ? 0 : parameter.StartIndex - 1).Take(parameter.Count);
                result = representations.ToList().Cast<SCIMRepresentation>();
            }

            if (parameter.SortBy != null)
            {
                var evaluatedExpression = parameter.SortBy.EvaluateOrderBy(
                    queryableRepresentations,
                    parameter.SortOrder ?? SearchSCIMRepresentationOrders.Descending);
                result = (IEnumerable<SCIMRepresentation>)evaluatedExpression.Compile().DynamicInvoke(queryableRepresentations);
            }

            result.FilterAttributes(parameter.IncludedAttributes, parameter.ExcludedAttributes);
            return Task.FromResult(new SearchSCIMRepresentationsResponse(totalResults, result));
        }
    }
}
