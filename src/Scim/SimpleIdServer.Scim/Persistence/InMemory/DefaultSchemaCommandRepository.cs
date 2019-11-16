// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Persistence.InMemory
{
    public class DefaultSchemaCommandRepository : InMemoryCommandRepository<SCIMSchema>, ISCIMSchemaCommandRepository
    {
        public DefaultSchemaCommandRepository(List<SCIMSchema> lstData) : base(lstData)
        {
        }
    }
}
