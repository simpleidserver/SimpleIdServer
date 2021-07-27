// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OpenID.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Persistence.InMemory
{
    public class InMemoryAuthenticationSchemeProviderRepository : IAuthenticationSchemeProviderRepository
    {
        private readonly ICollection<AuthenticationSchemeProvider> _authenticationSchemeProviders;

        public InMemoryAuthenticationSchemeProviderRepository(ICollection<AuthenticationSchemeProvider> authenticationSchemeProviders)
        {
            _authenticationSchemeProviders = authenticationSchemeProviders;
        }

        public Task<IEnumerable<AuthenticationSchemeProvider>> GetAll(CancellationToken cancellationToken)
        {
            IEnumerable<AuthenticationSchemeProvider> result = _authenticationSchemeProviders;
            return Task.FromResult(result);
        }
    }
}
