// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence
{
    public interface ISCIMAttributeMappingQueryRepository
    {
        Task<IEnumerable<SCIMAttributeMapping>> GetBySourceAttributes(IEnumerable<string> sourceAttributes);
        Task<IEnumerable<SCIMAttributeMapping>> GetBySourceResourceType(string sourceResourceType);
    }
}