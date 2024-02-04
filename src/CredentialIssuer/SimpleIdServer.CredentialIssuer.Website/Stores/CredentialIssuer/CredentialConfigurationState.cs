// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.CredentialIssuer.Domains;

namespace SimpleIdServer.CredentialIssuer.Website.Stores.CredentialIssuer;

[FeatureState]
public record CredentialConfigurationState
{
    public CredentialConfigurationState()
    {
        
    }

    public bool IsLoading { get; set; } = true;
    public CredentialConfiguration CredentialConfiguration { get; set; } = null;
}