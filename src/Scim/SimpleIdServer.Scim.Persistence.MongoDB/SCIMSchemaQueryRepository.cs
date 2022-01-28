// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Driver;
using SimpleIdServer.Scim.Domains;
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

        public async Task<SCIMSchema> FindSCIMSchemaById(string schemaId)
        {
            var collection = _scimDbContext.SCIMSchemaLst;
            var result = await collection.AsQueryable().Where(s => s.Id == schemaId).ToMongoFirstAsync();
            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async Task<IEnumerable<SCIMSchema>> FindSCIMSchemaByIdentifiers(IEnumerable<string> schemaIdentifiers)
        {
            var collection = _scimDbContext.SCIMSchemaLst;
            return await collection.AsQueryable().Where(s => schemaIdentifiers.Contains(s.Id)).ToMongoListAsync();
        }

        public async Task<SCIMSchema> FindRootSCIMSchemaByResourceType(string resourceType)
        {
            var collection = _scimDbContext.SCIMSchemaLst;
            var result = await collection.AsQueryable().Where(s => s.ResourceType == resourceType).ToMongoFirstAsync();
            if (result == null)
            {
                return null;
            }

            return result;
        }

        public Task<SCIMSchema> FindRootSCIMSchemaByName(string name)
        {
            var collection = _scimDbContext.SCIMSchemaLst;
            return collection.AsQueryable().Where(c => c.Name == name).ToMongoFirstAsync();
        }

        public async Task<IEnumerable<SCIMSchema>> GetAll()
        {
            var collection = _scimDbContext.SCIMSchemaLst;
            return await collection.AsQueryable().ToMongoListAsync();
        }

        public async Task<IEnumerable<SCIMSchema>> GetAllRoot()
        {
            var collection = _scimDbContext.SCIMSchemaLst;
            var result = await collection.AsQueryable().Where(s => s.IsRootSchema).ToMongoListAsync();
            return result;
        }
    }
}
