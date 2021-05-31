// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Persistence.InMemory;
using SimpleIdServer.OpenID.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Persistence.InMemory
{
    public class DefaultAuthenticationContextClassReferenceRepository : InMemoryCommandRepository<AuthenticationContextClassReference>, IAuthenticationContextClassReferenceRepository
    {
        public DefaultAuthenticationContextClassReferenceRepository(List<AuthenticationContextClassReference> acrLst) : base(acrLst)
        {
        }

        public Task<IEnumerable<AuthenticationContextClassReference>> FindACRByNames(IEnumerable<string> names, CancellationToken cancellationToken)
        {
            return Task.FromResult(LstData.Where(a => names.Contains(a.Name)));
        }

        public Task<ICollection<AuthenticationContextClassReference>> GetAllACR(CancellationToken token)
        {
            ICollection<AuthenticationContextClassReference> result = LstData;
            return Task.FromResult(result);
        }
    }
}
