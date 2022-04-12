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

        public override Task<bool> Update(SCIMRepresentation data, CancellationToken token)
        {
            var record = LstData.First(l => l.Id == data.Id);
            LstData.Remove(record);
            LstData.Add((SCIMRepresentation)data.Clone());
            return Task.FromResult(true);
        }
    }
}
