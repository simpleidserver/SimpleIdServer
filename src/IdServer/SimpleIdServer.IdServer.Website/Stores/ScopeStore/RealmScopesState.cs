// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.ScopeStore;

[FeatureState]
public record RealmScopesState
{
    public RealmScopesState()
    {
        
    }

    public RealmScopesState(bool isLoading, List<Domains.Scope> scopes)
    {
        IsLoading = isLoading;
        Scopes = scopes;
    }

    public bool IsLoading { get; set; } = true;
    public List<Domains.Scope> Scopes { get; set; } = new List<Domains.Scope>();
}
