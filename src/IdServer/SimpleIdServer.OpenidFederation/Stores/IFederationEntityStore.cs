// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.OpenidFederation.Domains;

namespace SimpleIdServer.OpenidFederation.Stores;

public interface IFederationEntityStore
{
    Task<List<FederationEntity>> GetAll(string realm, CancellationToken cancellationToken);
    Task<FederationEntity> Get(string sub, string realm, CancellationToken cancellationToken);
}