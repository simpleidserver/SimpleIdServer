// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using FormBuilder.Models.Layout;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.AcrsStore;

[FeatureState]
public record AcrState
{
    public AcrState() { }

    public AcrState(bool isLoading, AuthenticationContextClassReference acr)
    {
        Acr = acr;
        IsLoading = isLoading;
    }

    public AuthenticationContextClassReference Acr { get; set; } = null;
    public int Count { get; set; } = 0;
    public bool IsLoading { get; set; } = false;
}
