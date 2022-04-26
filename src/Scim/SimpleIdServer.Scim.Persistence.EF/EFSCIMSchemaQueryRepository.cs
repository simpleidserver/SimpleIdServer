// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class EFSCIMSchemaQueryRepository : ISCIMSchemaQueryRepository
    {
        private readonly SCIMQueryDbContext _context;

        public EFSCIMSchemaQueryRepository(SCIMQueryDbContext context)
        {
            _context = context;
        }

        public Task<SCIMSchema> FindSCIMSchemaById(string schemaId)
        {
            return _context.SCIMSchemaLst.Include(s => s.SchemaExtensions).Include(s => s.Attributes)
                .FirstOrDefaultAsync(s => s.Id == schemaId);
        }

        public async Task<IEnumerable<SCIMSchema>> FindSCIMSchemaByIdentifiers(IEnumerable<string> schemaIdentifiers)
        {
            var result = await _context.SCIMSchemaLst.Include(s => s.SchemaExtensions).Include(s => s.Attributes)
                .Where(s => schemaIdentifiers.Contains(s.Id))
                .ToListAsync();
            return result;

        }

        public async Task<SCIMSchema> FindRootSCIMSchemaByResourceType(string resourceType)
        {
            return await _context.SCIMSchemaLst.Include(s => s.SchemaExtensions).Include(s => s.Attributes)
                .FirstOrDefaultAsync(s => s.ResourceType == resourceType && s.IsRootSchema == true);
        }

        public async Task<IEnumerable<SCIMSchema>> GetAll()
        {
            var result = await _context.SCIMSchemaLst.Include(s => s.SchemaExtensions).Include(s => s.Attributes)
                .ToListAsync();
            return result;
        }

        public async Task<IEnumerable<SCIMSchema>> GetAllRoot()
        {
            var result = await _context.SCIMSchemaLst.
                .Include(s => s.SchemaExtensions)
                .Include(s => s.Attributes)
                .Where(s => s.IsRootSchema == true).ToListAsync();
            return result;
        }

        public Task<SCIMSchema> FindRootSCIMSchemaByName(string name)
        {
            return _context.SCIMSchemaLst.AsNoTracking().Include(s => s.SchemaExtensions).Include(s => s.Attributes)
                .FirstOrDefaultAsync(s => s.Name == name && s.IsRootSchema == true);
        }
    }
}
