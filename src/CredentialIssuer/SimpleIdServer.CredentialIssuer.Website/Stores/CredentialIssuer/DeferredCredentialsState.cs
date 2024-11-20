// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.CredentialIssuer.Domains;
using SimpleIdServer.CredentialIssuer.Store;

namespace SimpleIdServer.CredentialIssuer.Website.Stores.CredentialIssuer;

[FeatureState]
public record DeferredCredentialsState
{
    public DeferredCredentialsState()
    {
        
    }

    public SearchResult<DeferredCredential> DeferredCredentials { get; set; } = new SearchResult<DeferredCredential>();
    public bool IsLoading { get; set; } = true;
}