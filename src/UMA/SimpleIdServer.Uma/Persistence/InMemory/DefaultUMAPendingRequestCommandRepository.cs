// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Persistence.InMemory;
using SimpleIdServer.Uma.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.Uma.Persistence.InMemory
{
    public class DefaultUMAPendingRequestCommandRepository : InMemoryCommandRepository<UMAPendingRequest>, IUMAPendingRequestCommandRepository
    {
        public DefaultUMAPendingRequestCommandRepository(List<UMAPendingRequest> umaPendingRequests) : base(umaPendingRequests)
        {
        }
    }
}
