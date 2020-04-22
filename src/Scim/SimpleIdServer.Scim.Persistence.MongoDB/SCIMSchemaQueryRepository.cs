// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Persistence.MongoDB.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.MongoDB
{
    public class SCIMSchemaQueryRepository : ISCIMSchemaQueryRepository
    {
        private readonly IMongoDatabase _database;

        public SCIMSchemaQueryRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public Task<SCIMSchema> FindSCIMSchemaById(string schemaId)
        {
            var collection = _database.GetCollection<SCIMSchema>(Constants.CollectionNames.Schemas);
            return collection.AsQueryable().Where(s => s.Id == schemaId).ToMongoFirstAsync();
        }

        public async Task<IEnumerable<SCIMSchema>> FindSCIMSchemaByIdentifiers(IEnumerable<string> schemaIdentifiers)
        {
            var collection = _database.GetCollection<SCIMSchema>(Constants.CollectionNames.Schemas);
            var result = await collection.AsQueryable().Where(s => schemaIdentifiers.Contains(s.Id)).ToMongoListAsync();
            return result;

        }

        public Task<SCIMSchema> FindRootSCIMSchemaByResourceType(string resourceType)
        {
            var collection = _database.GetCollection<SCIMSchema>(Constants.CollectionNames.Schemas);
            return collection.AsQueryable().Where(s => s.ResourceType == resourceType).ToMongoFirstAsync();
        }

        public async Task<IEnumerable<SCIMSchema>> GetAll()
        {
            var collection = _database.GetCollection<SCIMSchema>(Constants.CollectionNames.Schemas);
            var result = await collection.AsQueryable().ToMongoListAsync();
            return result;
        }

        public async Task<IEnumerable<SCIMSchema>> GetAllRoot()
        {
            var collection = _database.GetCollection<SCIMSchema>(Constants.CollectionNames.Schemas);
            var result = await collection.AsQueryable().Where(s => s.IsRootSchema).ToMongoListAsync();
            return result;
        }
    }
}
