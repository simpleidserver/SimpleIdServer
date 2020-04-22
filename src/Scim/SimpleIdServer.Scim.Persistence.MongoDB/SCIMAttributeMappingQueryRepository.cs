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
    public class SCIMAttributeMappingQueryRepository : ISCIMAttributeMappingQueryRepository
    {
        private readonly IMongoDatabase _database;

        public SCIMAttributeMappingQueryRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<IEnumerable<SCIMAttributeMapping>> GetBySourceResourceType(string sourceResourceType)
        {
            var attributeMappings = _database.GetCollection<SCIMAttributeMapping>(Constants.CollectionNames.Mappings);
            var result = await attributeMappings.AsQueryable().Where(a => a.SourceResourceType == sourceResourceType).ToMongoListAsync();
            return result;
        }
    }
}