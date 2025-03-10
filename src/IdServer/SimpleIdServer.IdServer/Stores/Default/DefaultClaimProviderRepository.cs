// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultClaimProviderRepository : IClaimProviderRepository
{
    private readonly List<ClaimProvider> _claimProviders;

    public DefaultClaimProviderRepository(List<ClaimProvider> claimProviders)
    {
        _claimProviders = claimProviders;
    }

    public Task<List<ClaimProvider>> GetAll(CancellationToken cancellationToken)
        => Task.FromResult(_claimProviders);
}
