// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.InMemory
{
    public class DefaultSchemaCommandRepository : InMemoryCommandRepository<SCIMSchema>, ISCIMSchemaCommandRepository
    {
        public DefaultSchemaCommandRepository(List<SCIMSchema> lstData) : base(lstData)
        {
        }

        public Task<SCIMSchema> FindRootSCIMSchemaByResourceType(string resourceType)
        {
            var result = LstData.FirstOrDefault(s => s.ResourceType == resourceType && s.IsRootSchema == true);
            if (result == null)
            {
                return Task.FromResult(result);
            }

            return Task.FromResult((SCIMSchema)result.Clone());
        }

        public Task<IEnumerable<SCIMSchema>> FindSCIMSchemaByIdentifiers(IEnumerable<string> schemaIdentifiers)
        {
            var result = LstData.Where(s => schemaIdentifiers.Contains(s.Id));
            return Task.FromResult(result.Select(r => (SCIMSchema)r.Clone()));
        }

        public Task<IEnumerable<SCIMSchema>> FindSCIMSchemaByResourceTypes(IEnumerable<string> resourceTypes)
        {
            var result = LstData.Where(s => resourceTypes.Contains(s.ResourceType));
            return Task.FromResult(result.Select(r => (SCIMSchema)r.Clone()));
        }
    }
}
