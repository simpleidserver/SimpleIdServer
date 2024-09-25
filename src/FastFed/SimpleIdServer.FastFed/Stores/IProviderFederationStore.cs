// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.FastFed.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.Stores;

public interface IProviderFederationStore
{
    Task<List<IdentityProviderFederation>> GetAll(CancellationToken cancellationToken);
    Task<IdentityProviderFederation> Get(string entityId, CancellationToken cancellationToken);
    void Add(IdentityProviderFederation identityProviderFederation);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}