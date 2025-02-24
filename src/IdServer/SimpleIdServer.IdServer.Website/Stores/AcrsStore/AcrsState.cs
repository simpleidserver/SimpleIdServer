// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.AcrsStore;

[FeatureState]
public record AcrsState
{
    public AcrsState() { }

    public AcrsState(bool isLoading, IEnumerable<AuthenticationContextClassReference> apiResources)
    {
        Acrs = apiResources.Select(c => new SelectableAcr(c));
        Count = apiResources.Count();
        IsLoading = isLoading;
    }

    public IEnumerable<SelectableAcr>? Acrs { get; set; } = null;
    public int Count { get; set; } = 0;
    public bool IsLoading { get; set; } = false;
}

public class SelectableAcr
{
    public SelectableAcr(AuthenticationContextClassReference acr)
    {
        Value = acr;
    }

    public bool IsSelected { get; set; } = false;
    public bool IsNew { get; set; } = false;
    public AuthenticationContextClassReference Value { get; set; }
}
