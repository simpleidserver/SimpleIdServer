// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Persistence.EF.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class EFSCIMSchemaQueryRepository : ISCIMSchemaQueryRepository
    {
        private readonly SCIMDbContext _context;

        public EFSCIMSchemaQueryRepository(SCIMDbContext context)
        {
            _context = context;
        }

        public async Task<SCIMSchema> FindSCIMSchemaById(string schemaId)
        {
            var result = await _context.SCIMSchemaLst
                .Include(s => s.SchemaExtensions)
                .Include(s => s.Attributes).ThenInclude(s => s.SubAttributes).ThenInclude(s => s.SubAttributes)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == schemaId);
            if (result == null)
            {
                return null;
            }

            return result.ToDomain();
        }

        public async Task<IEnumerable<SCIMSchema>> FindSCIMSchemaByIdentifiers(IEnumerable<string> schemaIdentifiers)
        {
            var result = await _context.SCIMSchemaLst
                .Include(s => s.SchemaExtensions)
                .Include(s => s.Attributes).ThenInclude(s => s.SubAttributes).ThenInclude(s => s.SubAttributes)
                .AsNoTracking()
                .Where(s => schemaIdentifiers.Contains(s.Id)).ToListAsync();
            if (result == null)
            {
                return null;
            }

            return result.Select(r => r.ToDomain());

        }

        public async Task<SCIMSchema> FindRootSCIMSchemaByResourceType(string resourceType)
        {
            var result = await _context.SCIMSchemaLst
                .Include(s => s.SchemaExtensions)
                .Include(s => s.Attributes).ThenInclude(s => s.SubAttributes).ThenInclude(s => s.SubAttributes)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ResourceType == resourceType && s.IsRootSchema == true);
            if (result == null)
            {
                return null;
            }

            return result.ToDomain();
        }

        public async Task<IEnumerable<SCIMSchema>> GetAll()
        {
            var result = await _context.SCIMSchemaLst
                .Include(s => s.SchemaExtensions)
                .Include(s => s.Attributes).ThenInclude(s => s.SubAttributes).ThenInclude(s => s.SubAttributes)
                .AsNoTracking()
                .ToListAsync();
            return result.Select(r => r.ToDomain());
        }

        public async Task<IEnumerable<SCIMSchema>> GetAllRoot()
        {
            var result = await _context.SCIMSchemaLst
                .Include(s => s.SchemaExtensions)
                .Include(s => s.Attributes).ThenInclude(s => s.SubAttributes).ThenInclude(s => s.SubAttributes)
                .AsNoTracking()
                .Where(s => s.IsRootSchema == true).ToListAsync();
            return result.Select(r => r.ToDomain());
        }
    }
}
