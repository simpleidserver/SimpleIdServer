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
        
        private readonly SCIMDbContext _scimDbContext;

        public SCIMAttributeMappingQueryRepository(SCIMDbContext scimDbContext)
        {
            _scimDbContext = scimDbContext;
        }

        public async Task<IEnumerable<SCIMAttributeMapping>> GetBySourceResourceType(string sourceResourceType)
        {
            var attributeMappings = _scimDbContext.SCIMAttributeMappingLst;
            var result = await attributeMappings.AsQueryable().Where(a => a.SourceResourceType == sourceResourceType).ToMongoListAsync();
            return result;
        }
    }
}