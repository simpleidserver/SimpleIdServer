// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.InMemory
{
    public class DefaultSchemaQueryRepository : ISCIMSchemaQueryRepository
    {
        private readonly List<SCIMSchema> _schemas;

        public DefaultSchemaQueryRepository(List<SCIMSchema> schemas)
        {
            _schemas = schemas;
        }

        public Task<SCIMSchema> FindSCIMSchemaById(string schemaId)
        {
            return Task.FromResult(_schemas.FirstOrDefault(s => s.Id == schemaId));
        }

        public Task<IEnumerable<SCIMSchema>> FindSCIMSchemaByIdentifiers(IEnumerable<string> schemaIdentifiers)
        {
            return Task.FromResult(_schemas.Where(s => schemaIdentifiers.Contains(s.Id)));
        }

        public Task<IEnumerable<SCIMSchema>> GetAll()
        {
            return Task.FromResult((IEnumerable<SCIMSchema>)_schemas);
        }
    }
}
