// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using SimpleIdServer.IdServer.Api.AuthenticationMethods;

namespace SimpleIdServer.IdServer.Website.Stores.AuthMethodsStore;

[FeatureState]
public record UserLockingOptionsState
{
    public UserLockingOptionsState()
    {
        
    }
    public UserLockingOptionsState(bool isLoading, UserLockingResult options)
    {
        IsLoading = isLoading;
        Options = options;
    }

    public bool IsLoading {  get; set; }
    public UserLockingResult Options { get; set; }
}
