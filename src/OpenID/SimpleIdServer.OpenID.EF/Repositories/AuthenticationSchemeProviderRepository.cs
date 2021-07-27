// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Persistence;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.EF.Repositories
{
    public class AuthenticationSchemeProviderRepository : IAuthenticationSchemeProviderRepository
    {
        private readonly OpenIdDBContext _dbContext;

        public AuthenticationSchemeProviderRepository(OpenIdDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<AuthenticationSchemeProvider>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _dbContext.AuthenticationSchemeProviders.ToListAsync(cancellationToken);
            return result;
        }
    }
}
