// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.OpenidFederation.Domains;

namespace SimpleIdServer.OpenidFederation.Stores;

public interface IFederationEntityStore
{
    Task<FederationEntity?> Get(string id, CancellationToken cancellationToken);
    Task<FederationEntity?> GetByUrl(string realm, string url, CancellationToken cancellationToken);
    Task<List<FederationEntity>> GetAllSubordinates(string realm, CancellationToken cancellationToken);
    Task<List<FederationEntity>> GetAllAuthorities(string realm, CancellationToken cancellationToken);
    Task<FederationEntity> GetSubordinate(string sub, string realm, CancellationToken cancellationToken);
    Task<SearchResult<FederationEntity>> Search(string realm, SearchRequest request, CancellationToken cancellationToken);
    void Add(FederationEntity federationEntity);
    void Remove(FederationEntity federationEntity);
}