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
        
        private readonly SCIMDbContext _scimDbContext;

        public SCIMSchemaQueryRepository(SCIMDbContext scimDbContext)
        {
            _scimDbContext = scimDbContext;
        }

        public Task<SCIMSchema> FindSCIMSchemaById(string schemaId)
        {
            var collection = _scimDbContext.SCIMSchemaLst;
            return collection.AsQueryable().Where(s => s.Id == schemaId).ToMongoFirstAsync();
        }

        public async Task<IEnumerable<SCIMSchema>> FindSCIMSchemaByIdentifiers(IEnumerable<string> schemaIdentifiers)
        {
            var collection = _scimDbContext.SCIMSchemaLst;
            var result = await collection.AsQueryable().Where(s => schemaIdentifiers.Contains(s.Id)).ToMongoListAsync();
            return result;

        }

        public Task<SCIMSchema> FindRootSCIMSchemaByResourceType(string resourceType)
        {
            var collection = _scimDbContext.SCIMSchemaLst;
            return collection.AsQueryable().Where(s => s.ResourceType == resourceType).ToMongoFirstAsync();
        }

        public async Task<IEnumerable<SCIMSchema>> GetAll()
        {
            var collection = _scimDbContext.SCIMSchemaLst;
            var result = await collection.AsQueryable().ToMongoListAsync();
            return result;
        }

        public async Task<IEnumerable<SCIMSchema>> GetAllRoot()
        {
            var collection = _scimDbContext.SCIMSchemaLst;
            var result = await collection.AsQueryable().Where(s => s.IsRootSchema).ToMongoListAsync();
            return result;
        }
    }
}
