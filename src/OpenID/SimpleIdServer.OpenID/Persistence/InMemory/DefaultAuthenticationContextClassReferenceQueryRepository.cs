// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OpenID.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Persistence.InMemory
{
    public class DefaultAuthenticationContextClassReferenceQueryRepository : IAuthenticationContextClassReferenceQueryRepository
    {
        public List<AuthenticationContextClassReference> _acrLst;

        public DefaultAuthenticationContextClassReferenceQueryRepository(List<AuthenticationContextClassReference> acrLst)
        {
            _acrLst = acrLst;
        }

        public Task<IEnumerable<AuthenticationContextClassReference>> FindACRByNames(IEnumerable<string> names, CancellationToken cancellationToken)
        {
            return Task.FromResult(_acrLst.Where(a => names.Contains(a.Name)));
        }

        public Task<ICollection<AuthenticationContextClassReference>> GetAllACR(CancellationToken token)
        {
            ICollection<AuthenticationContextClassReference> result = _acrLst;
            return Task.FromResult(result);
        }
    }
}
