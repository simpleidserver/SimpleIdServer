// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.CredentialIssuer.Domains;

namespace SimpleIdServer.CredentialIssuer.Website.Stores.CredentialIssuer;

[FeatureState]
public record DeferredCredentialState
{
    public DeferredCredentialState()
    {

    }

    public DeferredCredential DeferredCredential { get; set; } = null;
    public bool IsLoading { get; set; } = true;
}