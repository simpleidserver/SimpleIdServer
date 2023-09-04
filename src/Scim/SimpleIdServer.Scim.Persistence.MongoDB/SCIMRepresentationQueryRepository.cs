// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
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
        private readonly MongoDbOptions _options;

        public SCIMRepresentationQueryRepository(SCIMDbContext scimDbContext, IOptions<MongoDbOptions> options)
        {
            _scimDbContext = scimDbContext;
            _options = options.Value;
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

        public async Task<SearchSCIMRepresentationsResponse> FindSCIMRepresentations(SearchSCIMRepresentationsParameter parameter)
        {
            IQueryable<EnrichedRepresentation> representationAttributes = from a in _scimDbContext.SCIMRepresentationLst.AsQueryable()
                join b in _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable() on a.Id equals b.RepresentationId into FlatAttributes
                select new EnrichedRepresentation
                {
                    Representation = a,
                    FlatAttributes = FlatAttributes
                };
            var queryableRepresentations = representationAttributes.Where(s => s.Representation.ResourceType == parameter.ResourceType);
            if(parameter.SortBy == null)
                queryableRepresentations = queryableRepresentations.OrderBy(s => s.Representation.Id);

            if (parameter.Filter != null)
            {
                var evaluatedExpression = parameter.Filter.Evaluate(queryableRepresentations);
                queryableRepresentations = evaluatedExpression.Compile().DynamicInvoke(queryableRepresentations) as IMongoQueryable<EnrichedRepresentation>;
            }

            if (parameter.SortBy != null)
            {
                var evaluatedExpression = parameter.SortBy.EvaluateMongoDbOrderBy(
                    queryableRepresentations,
                    parameter.SortOrder ?? SearchSCIMRepresentationOrders.Descending);
                var orderedResult = evaluatedExpression.Compile().DynamicInvoke(queryableRepresentations) as IEnumerable<EnrichedRepresentation>;
                int total = orderedResult.Count();
                orderedResult = orderedResult.Skip(parameter.StartIndex <= 1 ? 0 : parameter.StartIndex - 1).Take(parameter.Count);
                var result = orderedResult.ToList();
                foreach (var record in result) record.Representation.FlatAttributes = record.FlatAttributes.ToList();
                var representations = result.Select(r => r.Representation);
                representations.FilterAttributes(parameter.IncludedAttributes, parameter.ExcludedAttributes);
                return new SearchSCIMRepresentationsResponse(total, representations);
            }
            else
            {
                int total = queryableRepresentations.Count();
                queryableRepresentations = queryableRepresentations.Skip(parameter.StartIndex <= 1 ? 0 : parameter.StartIndex - 1).Take(parameter.Count);
                var result = await queryableRepresentations.ToMongoListAsync();
                foreach (var record in result) record.Representation.FlatAttributes = record.FlatAttributes.ToList();
                var representations = result.Select(r => r.Representation);
                representations.FilterAttributes(parameter.IncludedAttributes, parameter.ExcludedAttributes);
                return new SearchSCIMRepresentationsResponse(total, representations);
            }
        }
    }
}
