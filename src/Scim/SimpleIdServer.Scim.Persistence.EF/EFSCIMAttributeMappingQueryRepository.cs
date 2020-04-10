// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Persistence.EF.Extensions;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class EFSCIMAttributeMappingQueryRepository : ISCIMAttributeMappingQueryRepository
    {
        private readonly SCIMDbContext _scimDbContext;

        public EFSCIMAttributeMappingQueryRepository(SCIMDbContext scimDbContext)
        {
            _scimDbContext = scimDbContext;
        }

        public async Task<IEnumerable<SCIMAttributeMapping>> GetBySourceResourceType(string sourceResourceType)
        {
            var result = await _scimDbContext.SCIMAttributeMappingLst.Where(a => a.SourceResourceType == sourceResourceType).ToListAsync();
            return result.Select(r => r.ToDomain());
        }
    }
}
