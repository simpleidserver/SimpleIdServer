// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using SimpleIdServer.IdServer.Api.AuthenticationMethods;

namespace SimpleIdServer.IdServer.Website.Stores.AuthMethodsStore;

[FeatureState]
public record AuthMethodState
{
    public AuthMethodState()
    {

    }

    public AuthMethodState(AuthenticationMethodResult authenticationMethod, bool isLoading)
    {
        AuthenticationMethod = authenticationMethod;
        IsLoading = isLoading;
    }

    public AuthenticationMethodResult AuthenticationMethod { get; set; }
    public bool IsLoading { get; set; } = true;
}
