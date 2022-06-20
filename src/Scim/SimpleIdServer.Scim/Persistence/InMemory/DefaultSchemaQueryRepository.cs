// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
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

        public Task<SCIMSchema> FindRootSCIMSchemaByResourceType(string resourceType)
        {
            var result = _schemas.FirstOrDefault(s => s.ResourceType == resourceType && s.IsRootSchema == true);
            if (result == null)
            {
                return Task.FromResult(result);
            }

            return Task.FromResult((SCIMSchema)result.Clone());
        }

        public Task<IEnumerable<SCIMSchema>> FindSCIMSchemaByIdentifiers(IEnumerable<string> schemaIdentifiers)
        {
            var result = _schemas.Where(s => schemaIdentifiers.Contains(s.Id));
            return Task.FromResult(result.Select(r => (SCIMSchema)r.Clone()));
        }

        public Task<SCIMSchema> FindSCIMSchemaById(string schemaId)
        {
            var result = _schemas.FirstOrDefault(s => s.Id == schemaId);
            if (result == null)
            {
                return Task.FromResult(result);
            }

            return Task.FromResult((SCIMSchema)result.Clone());
        }

        public Task<IEnumerable<SCIMSchema>> GetAll()
        {
            return Task.FromResult((IEnumerable<SCIMSchema>)_schemas.Select(s => (SCIMSchema)s.Clone()));
        }

        public Task<IEnumerable<SCIMSchema>> GetAllRoot()
        {
            return Task.FromResult((IEnumerable<SCIMSchema>)_schemas.Where(s => s.IsRootSchema == true).Select(s => (SCIMSchema)s.Clone()));
        }

        public Task<SCIMSchema> FindRootSCIMSchemaByName(string name)
        {
            return Task.FromResult(_schemas.FirstOrDefault(s => s.Name == name));
        }

        public Task<IEnumerable<SCIMSchema>> FindSCIMSchemaByResourceTypes(IEnumerable<string> resourceTypes)
        {
            var result = _schemas.Where(s => resourceTypes.Contains(s.ResourceType));
            return Task.FromResult(result.Select(r => (SCIMSchema)r.Clone()));
        }
    }
}
