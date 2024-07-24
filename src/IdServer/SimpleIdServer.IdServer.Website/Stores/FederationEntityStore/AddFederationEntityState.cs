// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.FederationEntityStore;

[FeatureState]
public record AddFederationEntityState
{
    public AddFederationEntityState() { }

    public AddFederationEntityState(bool isAdding)
    {
        IsAdding = isAdding;
    }

    public bool IsAdding { get; set; } = false;
    public string ErrorMessage { get; set; }
}
