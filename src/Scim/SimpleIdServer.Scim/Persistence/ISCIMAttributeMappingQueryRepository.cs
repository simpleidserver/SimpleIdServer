// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence
{
    public interface ISCIMAttributeMappingQueryRepository
    {
        Task<List<SCIMAttributeMapping>> GetBySourceAttributes(IEnumerable<string> sourceAttributes);
        Task<List<SCIMAttributeMapping>> GetBySourceResourceType(string sourceResourceType);
        Task<List<SCIMAttributeMapping>> GetAll();
    }
}