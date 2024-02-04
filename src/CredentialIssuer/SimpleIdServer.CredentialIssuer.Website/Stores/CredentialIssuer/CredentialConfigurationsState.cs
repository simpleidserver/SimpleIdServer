// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.CredentialIssuer.Domains;

namespace SimpleIdServer.CredentialIssuer.Website.Stores.CredentialIssuer;

[FeatureState]
public record CredentialConfigurationsState
{
    public CredentialConfigurationsState()
    {

    }

    public List<SelectableCredentialConfiguration> CredentialConfigurations { get; set; } = null;
    public bool IsLoading { get; set; } = true;
}

public class SelectableCredentialConfiguration
{
    public SelectableCredentialConfiguration(CredentialConfiguration credentialConfiguration)
    {
        CredentialConfiguration = credentialConfiguration;
        IsSelected = false;
        IsNew = false;
    }

    public CredentialConfiguration CredentialConfiguration { get; set; }
    public bool IsNew { get; set; }
    public bool IsSelected { get; set; }
}