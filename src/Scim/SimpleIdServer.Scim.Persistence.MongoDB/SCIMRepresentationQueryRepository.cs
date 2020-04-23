// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Driver;
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
            var collection = _scimDbContext.SCIMRepresentationLst;
            var record = await collection.AsQueryable()
                .Where(r => (endpoint == null || endpoint == r.ResourceType)
                    && r.Attributes.Any(a => (a.SchemaAttribute.Id == schemaAttributeId && a.ValuesString.Contains(value))
                        || (a.Children.Any(sa => sa.SchemaAttribute.Id == schemaAttributeId && sa.ValuesString.Contains(value) ||
                            (sa.Children.Any(ssa => ssa.SchemaAttribute.Id == schemaAttributeId && ssa.ValuesString.Contains(value))))
                    )
                 ))
                .ToMongoFirstAsync();
            if (record == null)
            {
                return null;
            }

            return record.ToDomain(_scimDbContext.Database);
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string schemaAttributeId, int value, string endpoint = null)
        {
            var collection = _scimDbContext.SCIMRepresentationLst;
            var record = await collection.AsQueryable()
                .Where(r => (endpoint == null || endpoint == r.ResourceType) 
                    && r.Attributes.Any(a => (a.SchemaAttribute.Id == schemaAttributeId && a.ValuesInteger.Contains(value))
                        || (a.Children.Any(sa => sa.SchemaAttribute.Id == schemaAttributeId && sa.ValuesInteger.Contains(value) ||
                            (sa.Children.Any(ssa => ssa.SchemaAttribute.Id == schemaAttributeId && ssa.ValuesInteger.Contains(value))))
                    )
                 ))
                .ToMongoFirstAsync();
            if (record == null)
            {
                return null;
            }

            return record.ToDomain(_scimDbContext.Database);
        }

        public async Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationByAttributes(string schemaAttributeId, IEnumerable<string> values, string endpoint = null)
        {
            var collection = _scimDbContext.SCIMRepresentationLst;
            var records = await collection.AsQueryable()
                .Where(r => (endpoint == null || endpoint == r.ResourceType)
                    && r.Attributes.Any(a => (a.SchemaAttribute.Id == schemaAttributeId && a.ValuesString.Any(v => values.Contains(v)))
                        || (a.Children.Any(sa => sa.SchemaAttribute.Id == schemaAttributeId && sa.ValuesString.Any(v => values.Contains(v)) ||
                            (sa.Children.Any(ssa => ssa.SchemaAttribute.Id == schemaAttributeId && ssa.ValuesString.Any(v => values.Contains(v)))))
                    )
                 )).ToMongoListAsync();
            return records.Select(_ => _.ToDomain(_scimDbContext.Database));
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId)
        {
            var collection = _scimDbContext.SCIMRepresentationLst;
            var result = await collection.AsQueryable().Where(a => a.Id == representationId).ToMongoFirstAsync();
            if (result == null)
            {
                return null;
            }

            return result.ToDomain(_scimDbContext.Database);
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId, string resourceType)
        {
            var collection = _scimDbContext.SCIMRepresentationLst;
            var result = await collection.AsQueryable().Where(a => a.Id == representationId && a.ResourceType == resourceType).ToMongoFirstAsync();
            if (result == null)
            {
                return null;
            }

            return result.ToDomain(_scimDbContext.Database);
        }

        public Task<SearchSCIMRepresentationsResponse> FindSCIMRepresentations(SearchSCIMRepresentationsParameter parameter)
        {
            var collection = _scimDbContext.SCIMRepresentationLst;
            var queryableRepresentations = collection.AsQueryable().Where(s => s.ResourceType == parameter.ResourceType);
            if (parameter.Filter != null)
            {
                var evaluatedExpression = parameter.Filter.Evaluate(queryableRepresentations);
                queryableRepresentations = (IQueryable<SCIMRepresentationModel>)evaluatedExpression.Compile().DynamicInvoke(queryableRepresentations);
            }

            int totalResults = queryableRepresentations.Count();
            var result = queryableRepresentations.Skip(parameter.StartIndex).Take(parameter.Count).ToList().Select(s => s.ToDomain(_scimDbContext.Database));
            return Task.FromResult(new SearchSCIMRepresentationsResponse(totalResults, result));
        }
    }
}
