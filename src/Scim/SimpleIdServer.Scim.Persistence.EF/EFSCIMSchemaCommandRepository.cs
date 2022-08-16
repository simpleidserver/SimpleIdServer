// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class EFSCIMSchemaCommandRepository : ISCIMSchemaCommandRepository
    {
        private readonly SCIMDbContext _context;

        public EFSCIMSchemaCommandRepository(SCIMDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SCIMSchema>> FindSCIMSchemaByIdentifiers(IEnumerable<string> schemaIdentifiers)
        {
            var result = await _context.SCIMSchemaLst.Include(s => s.SchemaExtensions).Include(s => s.Attributes)
                .Where(s => schemaIdentifiers.Contains(s.Id))
                .ToListAsync();
            return result;
        }

        public async Task<IEnumerable<SCIMSchema>> FindSCIMSchemaByResourceTypes(IEnumerable<string> resourceTypes)
        {
            var result = await _context.SCIMSchemaLst
                .Include(s => s.SchemaExtensions)
                .Include(s => s.Attributes)
                .Where(s => resourceTypes.Contains(s.ResourceType))
                .ToListAsync();
            return result;
        }

        public async Task<SCIMSchema> FindRootSCIMSchemaByResourceType(string resourceType)
        {
            return await _context.SCIMSchemaLst.Include(s => s.SchemaExtensions).Include(s => s.Attributes)
                .FirstOrDefaultAsync(s => s.ResourceType == resourceType && s.IsRootSchema == true);
        }
    }
}