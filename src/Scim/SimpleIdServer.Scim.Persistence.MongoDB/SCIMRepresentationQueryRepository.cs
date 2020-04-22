// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Persistence.MongoDB.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.MongoDB
{
    public class SCIMRepresentationQueryRepository : ISCIMRepresentationQueryRepository
    {
        private readonly IMongoDatabase _database;

        public SCIMRepresentationQueryRepository(IOptions<MongoDbOptions> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            _database = client.GetDatabase(options.Value.Database);
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string schemaAttributeId, string value, string endpoint = null)
        {            
            var collection = _database.GetCollection<SCIMRepresentation>(Constants.CollectionNames.Representations);
            var record = await collection.AsQueryable()
                .Where(r => (endpoint == null || endpoint == r.ResourceType)
                    && r.Attributes.Any(a => (a.SchemaAttribute.Id == schemaAttributeId && a.ValuesString.Contains(value))
                        || (a.Values.Any(sa => sa.SchemaAttribute.Id == schemaAttributeId && sa.ValuesString.Contains(value) ||
                            (sa.Values.Any(ssa => ssa.SchemaAttribute.Id == schemaAttributeId && ssa.ValuesString.Contains(value))))
                    )
                 ))
                .ToMongoFirstAsync();
            if (record == null)
            {
                return null;
            }

            return record;
        }

        public async Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string schemaAttributeId, int value, string endpoint = null)
        {
            var collection = _database.GetCollection<SCIMRepresentation>(Constants.CollectionNames.Representations);
            var record = await collection.AsQueryable()
                .Where(r => (endpoint == null || endpoint == r.ResourceType) 
                    && r.Attributes.Any(a => (a.SchemaAttribute.Id == schemaAttributeId && a.ValuesInteger.Contains(value))
                        || (a.Values.Any(sa => sa.SchemaAttribute.Id == schemaAttributeId && sa.ValuesInteger.Contains(value) ||
                            (sa.Values.Any(ssa => ssa.SchemaAttribute.Id == schemaAttributeId && ssa.ValuesInteger.Contains(value))))
                    )
                 ))
                .ToMongoFirstAsync();
            if (record == null)
            {
                return null;
            }

            return record;
        }

        public async Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationByAttributes(string schemaAttributeId, IEnumerable<string> values, string endpoint = null)
        {
            var collection = _database.GetCollection<SCIMRepresentation>(Constants.CollectionNames.Representations);
            var records = await collection.AsQueryable()
                .Where(r => (endpoint == null || endpoint == r.ResourceType) 
                    && r.Attributes.Any(a => (a.SchemaAttribute.Id == schemaAttributeId && a.ValuesString.Any(v => values.Contains(v)))
                        || (a.Values.Any(sa => sa.SchemaAttribute.Id == schemaAttributeId && sa.ValuesString.Any(v => values.Contains(v)) ||
                            (sa.Values.Any(ssa => ssa.SchemaAttribute.Id == schemaAttributeId && ssa.ValuesString.Any(v => values.Contains(v)))))
                    )
                 ))
                .ToMongoListAsync();
            return records;
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId)
        {
            var collection = _database.GetCollection<SCIMRepresentation>(Constants.CollectionNames.Representations);
            return collection.AsQueryable().Where(a => a.Id == representationId).ToMongoFirstAsync();
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationById(string representationId, string resourceType)
        {
            var collection = _database.GetCollection<SCIMRepresentation>(Constants.CollectionNames.Representations);
            return collection.AsQueryable().Where(a => a.Id == representationId && a.ResourceType == resourceType).ToMongoFirstAsync();
        }

        public Task<SearchSCIMRepresentationsResponse> FindSCIMRepresentations(SearchSCIMRepresentationsParameter parameter)
        {
            var collection = _database.GetCollection<SCIMRepresentation>(Constants.CollectionNames.Representations);
            var queryableRepresentations = collection.AsQueryable().Where(s => s.ResourceType == parameter.ResourceType);
            if (parameter.Filter != null)
            {
                var evaluatedExpression = parameter.Filter.Evaluate(queryableRepresentations);
                queryableRepresentations = (IQueryable<SCIMRepresentation>)evaluatedExpression.Compile().DynamicInvoke(queryableRepresentations);
            }

            int totalResults = queryableRepresentations.Count();
            var result = queryableRepresentations.Skip(parameter.StartIndex).Take(parameter.Count).ToList();
            return Task.FromResult(new SearchSCIMRepresentationsResponse(totalResults, result));
        }
    }
}
