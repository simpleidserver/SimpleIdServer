// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;

namespace SimpleIdServer.CredentialIssuer.Website.Stores.CredentialIssuer;

[FeatureState]
public record CredentialsState
{
    public CredentialsState()
    {
        
    }

    public List<Domains.Credential> Credentials { get; set; } = null;
    public bool IsLoading { get; set; } = true;
}
