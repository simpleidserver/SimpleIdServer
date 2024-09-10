// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.FastFed.ApplicationProvider.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.ApplicationProvider.Stores;

public interface IIdentityProviderFederationStore
{
    Task<IdentityProviderFederation> Get(string entityId, CancellationToken cancellationToken);
    void Add(IdentityProviderFederation identityProviderFederation);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}