// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Domains.ACRs
{
    public class DefaultAuthenticationContextClassReferenceQueryRepository : IAuthenticationContextClassReferenceQueryRepository
    {
        public List<AuthenticationContextClassReference> _acrLst;

        public DefaultAuthenticationContextClassReferenceQueryRepository(List<AuthenticationContextClassReference> acrLst)
        {
            _acrLst = acrLst;
        }

        public Task<AuthenticationContextClassReference> FindACRByName(string name)
        {
            return Task.FromResult(_acrLst.FirstOrDefault(a => a.Name == name));
        }
    }
}
