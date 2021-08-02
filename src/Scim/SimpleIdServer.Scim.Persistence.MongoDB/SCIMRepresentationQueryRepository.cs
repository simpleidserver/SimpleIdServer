// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Persistence.MongoDB.Extensions;
using SimpleIdServer.Scim.Persistence.MongoDB.Models;
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

        public async Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string schemaAttributeId, string value, string endpoint = null)
        {            
            var attr = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => (endpoint == null || endpoint == a.RepresentationResourceType) && a.SchemaAttributeId == schemaAttributeId && a.ValueString == value)
                .ToMongoFirstAsync();
            if (attr == null)
            {
                return null;
            }

            var result = await _scimDbContext.SCIMRepresentationLst.AsQueryable().Where(a => a.Id == attr.RepresentationId).ToMongoFirstAsync();
            result.Init(_scimDbContext.Database);
            return result;
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string schemaAttributeId, int value, string endpoint = null)
        {
            var attr = await _scimDbContext.SCIMRepresentationAttributeLst.AsQueryable()
                .Where(a => (endpoint == null || endpoint == a.RepresentationResourceType) && a.SchemaAttributeId == schemaAttributeId && a.ValueInteger == value)
                .ToMongoFirstAsync();
            if (attr == null)
            {
                return null;
            }

            var result = await _scimDbContext.SCIMRepresentationLst.AsQueryable().Where(a => a.Id == attr.RepresentationId).ToMongoFirstAsync();
            result.Init(_scimDbContext.Database);
            return result;
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId)
        {
            var collection = _scimDbContext.SCIMRepresentationLst;
            var result = await collection.AsQueryable().Where(a => a.Id == representationId).ToMongoFirstAsync();
            if (result == null)
            {
                return null;
            }

            result.Init(_scimDbContext.Database);
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

            result.Init(_scimDbContext.Database);
            return result;
        }

        public async Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationByIds(IEnumerable<string> representationIds, string resourceType)
        {
            var result = await _scimDbContext.SCIMRepresentationLst.AsQueryable()
                .Where(r => r.ResourceType == resourceType && representationIds.Contains(r.Id))
                .ToMongoListAsync<SCIMRepresentationModel>();
            foreach(var representation in result)
            {
                representation.Init(_scimDbContext.Database);
            }

            return result;
        }

        public Task<SearchSCIMRepresentationsResponse> FindSCIMRepresentations(SearchSCIMRepresentationsParameter parameter)
        {
            IEnumerable<SCIMRepresentation> result = new List<SCIMRepresentation>();
            var collection = _scimDbContext.SCIMRepresentationLst;
            var queryableRepresentations = collection.AsQueryable().Where(s => s.ResourceType == parameter.ResourceType);
            if (parameter.Filter != null)
            {
                var evaluatedExpression = parameter.Filter.Evaluate(queryableRepresentations);
                var filtered = evaluatedExpression.Compile().DynamicInvoke(queryableRepresentations) as IMongoQueryable<SCIMRepresentation>;
                int totalResults = filtered.Count();
                if (parameter.Count > 0)
                {
                    result = filtered.Skip(parameter.StartIndex).Take(parameter.Count).ToList();
                }

                return Task.FromResult(new SearchSCIMRepresentationsResponse(totalResults, result));
            }
            else
            {
                int totalResults = queryableRepresentations.Count();
                if (parameter.Count > 0)
                {
                    result = queryableRepresentations.Skip(parameter.StartIndex).Take(parameter.Count).ToList();
                }

                return Task.FromResult(new SearchSCIMRepresentationsResponse(totalResults, result));
            }
        }
    }
}
