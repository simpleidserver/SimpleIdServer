// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleIdServer.Scim.Domain;

namespace SimpleIdServer.Scim.Persistence.InMemory
{
    public class DefaultAttributeMappingQueryRepository : ISCIMAttributeMappingQueryRepository
    {
        private readonly List<SCIMAttributeMapping> _attributeMappingLst;

        public DefaultAttributeMappingQueryRepository(List<SCIMAttributeMapping> attributeMappingLst)
        {
            _attributeMappingLst = attributeMappingLst;
        }

        public Task<IEnumerable<SCIMAttributeMapping>> GetBySourceResourceType(string sourceResourceType)
        {
            return Task.FromResult(_attributeMappingLst.Where(a => a.SourceResourceType == sourceResourceType));
        }
    }
}
