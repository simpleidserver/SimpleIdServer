// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OpenID.Domains;

namespace SimpleIdServer.OpenID.Store
{
    public interface IAuthenticationContextClassReferenceRepository
    {
        IQueryable<AuthenticationContextClassReference> Query();
    }

    public class AuthenticationContextClassReferenceRepository : IAuthenticationContextClassReferenceRepository
    {
        public IQueryable<AuthenticationContextClassReference> Query()
        {
            throw new NotImplementedException();
        }
    }
}
