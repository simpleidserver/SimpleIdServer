// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OpenID.Domains;

namespace SimpleIdServer.OpenID.Store
{
    public interface IAuthenticationSchemeProviderRepository
    {
        IQueryable<AuthenticationSchemeProvider> Query();
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class AuthenticationSchemeProviderRepository : IAuthenticationSchemeProviderRepository
    {
        public IQueryable<AuthenticationSchemeProvider> Query()
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
