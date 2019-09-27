// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Persistence.InMemory;
using SimpleIdServer.OpenID.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.OpenID.Persistence.InMemory
{
    public class DefaultAuthenticationContextClassReferenceCommandRepository : InMemoryCommandRepository<AuthenticationContextClassReference>, IAuthenticationContextClassReferenceCommandRepository
    {
        public DefaultAuthenticationContextClassReferenceCommandRepository(List<AuthenticationContextClassReference> acrs) : base(acrs)
        {
        }
    }
}
