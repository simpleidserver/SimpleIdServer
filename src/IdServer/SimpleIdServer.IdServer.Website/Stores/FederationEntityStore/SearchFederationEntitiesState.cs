// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using SimpleIdServer.OpenidFederation.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.FederationEntityStore;

[FeatureState]
public record SearchFederationEntitiesState
{
    public SearchFederationEntitiesState()
    {
        
    }

    public SearchFederationEntitiesState(bool isLoading, IEnumerable<FederationEntity> federationEntities, int nb)
    {
        FederationEntities = federationEntities.Select(g => new SelectableFederationEntity(g));
        IsLoading = isLoading;
        Count = nb;
    }

    public IEnumerable<SelectableFederationEntity>? FederationEntities { get; set; } = null;
    public int Count { get; set; } = 0;
    public bool IsLoading { get; set; } = true;
}

public class SelectableFederationEntity
{
    public SelectableFederationEntity(FederationEntity federationEntity)
    {
        FederationEntity = federationEntity;
    }

    public bool IsSelected { get; set; } = false;
    public bool IsNew { get; set; } = false;
    public FederationEntity FederationEntity { get; set; }

}
