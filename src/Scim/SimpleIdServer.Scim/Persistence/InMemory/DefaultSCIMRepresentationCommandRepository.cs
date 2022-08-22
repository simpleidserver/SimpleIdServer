// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.InMemory
{
    public class DefaultSCIMRepresentationCommandRepository : InMemoryCommandRepository<SCIMRepresentation>, ISCIMRepresentationCommandRepository
    {
        public DefaultSCIMRepresentationCommandRepository(List<SCIMRepresentation> lstData) : base(lstData)
        {
        }

        public Task<IEnumerable<SCIMRepresentation>> FindSCIMRepresentationByIds(IEnumerable<string> representationIds, string resourceType)
        {
            IEnumerable<SCIMRepresentation> representations = LstData.AsQueryable().Where(r => r.ResourceType == resourceType && representationIds.Contains(r.Id));
            return Task.FromResult(representations);
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string attrSchemaId, string value, string endpoint = null)
        {
            var result = LstData.FirstOrDefault(r => (endpoint == null || endpoint == r.ResourceType) && r.FlatAttributes.Any(a => a.SchemaAttribute.Id == attrSchemaId && a.ValueString == value));
            if (result == null)
            {
                return Task.FromResult(result);
            }

            return Task.FromResult((SCIMRepresentation)result.Clone());
        }

        public Task<SCIMRepresentation> FindSCIMRepresentationByAttribute(string attrSchemaId, int value, string endpoint = null)
        {
            var result = LstData.FirstOrDefault(r => (endpoint == null || endpoint == r.ResourceType) && r.FlatAttributes.Any(a => a.SchemaAttribute.Id == attrSchemaId && a.ValueInteger == value));
            if (result == null)
            {
                return Task.FromResult(result);
            }

            return Task.FromResult(result);
        }

        public override Task<bool> Update(SCIMRepresentation data, CancellationToken token)
        {
            var record = LstData.First(l => l.Id == data.Id);
            LstData.Remove(record);
            LstData.Add((SCIMRepresentation)data.Clone());
            return Task.FromResult(true);
        }
    }
}
