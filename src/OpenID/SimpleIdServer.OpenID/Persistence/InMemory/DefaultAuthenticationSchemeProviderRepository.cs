// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OpenID.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Persistence.InMemory
{
    public class DefaultAuthenticationSchemeProviderRepository : IAuthenticationSchemeProviderRepository
    {
        private readonly ICollection<AuthenticationSchemeProvider> _authenticationSchemeProviders;

        public DefaultAuthenticationSchemeProviderRepository(ICollection<AuthenticationSchemeProvider> authenticationSchemeProviders)
        {
            _authenticationSchemeProviders = authenticationSchemeProviders;
        }

        public Task<AuthenticationSchemeProvider> Get(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_authenticationSchemeProviders.FirstOrDefault(a => a.Id == id));
        }

        public Task<IEnumerable<AuthenticationSchemeProvider>> GetAll(CancellationToken cancellationToken)
        {
            IEnumerable<AuthenticationSchemeProvider> result = _authenticationSchemeProviders;
            return Task.FromResult(result);
        }

        public Task<bool> Update(AuthenticationSchemeProvider authenticationSchemeProvider, CancellationToken cancellationToken)
        {
            _authenticationSchemeProviders.Remove(_authenticationSchemeProviders.First(a => a.Id == authenticationSchemeProvider.Id));
            _authenticationSchemeProviders.Add((AuthenticationSchemeProvider)authenticationSchemeProvider.Clone());
            return Task.FromResult(true);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }
}
