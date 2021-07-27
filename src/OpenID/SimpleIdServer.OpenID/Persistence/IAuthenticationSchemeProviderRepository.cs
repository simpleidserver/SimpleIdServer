// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OpenID.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Persistence
{
    public interface IAuthenticationSchemeProviderRepository
    {
        Task<IEnumerable<AuthenticationSchemeProvider>> GetAll(CancellationToken cancellationToken);
    }
}
