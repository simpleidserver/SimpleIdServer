// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class EFSCIMAttributeMappingQueryRepository : ISCIMAttributeMappingQueryRepository
    {
        private readonly SCIMDbContext _scimDbContext;

        public EFSCIMAttributeMappingQueryRepository(SCIMDbContext scimDbContext)
        {
            _scimDbContext = scimDbContext;
        }

        public async Task<List<SCIMAttributeMapping>> GetAll()
        {
            var result = await _scimDbContext.SCIMAttributeMappingLst.ToListAsync();
            return result;
        }

        public async Task<List<SCIMAttributeMapping>> GetBySourceAttributes(IEnumerable<string> sourceAttributes)
        {
            var result = await _scimDbContext.SCIMAttributeMappingLst.Where(a => sourceAttributes.Contains(a.SourceAttributeSelector)).ToListAsync();
            return result;
        }

        public async Task<List<SCIMAttributeMapping>> GetBySourceResourceType(string sourceResourceType)
        {
            var result = await _scimDbContext.SCIMAttributeMappingLst.Where(a => a.SourceResourceType == sourceResourceType).ToListAsync();
            return result;
        }

        public async Task<List<SCIMAttributeMapping>> GetByTargetResourceType(string targetResourceType)
        {
            var result = await _scimDbContext.SCIMAttributeMappingLst.Where(a => a.TargetResourceType == targetResourceType).ToListAsync();
            return result;
        }
    }
}