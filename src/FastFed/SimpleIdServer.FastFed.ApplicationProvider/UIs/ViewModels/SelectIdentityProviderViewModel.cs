// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.FastFed.ApplicationProvider.Models;

namespace SimpleIdServer.FastFed.ApplicationProvider.UIs.ViewModels;

public class SelectIdentityProviderViewModel
{
    public SelectIdentityProviderViewModel(string errorCode, string errorDescription)
    {
        ErrorCode = errorCode;
        ErrorDescription = errorDescription;
    }

    public SelectIdentityProviderViewModel(string url, FastFed.Models.ProviderMetadata providerMetadata, IdentityProviderFederation identityProviderFederation)
    {
        Url = url;
        ProviderMetadata = providerMetadata;
        IdentityProviderFederation = identityProviderFederation;
    }

    public string ErrorCode { get; set; }
    public string ErrorDescription { get; set; }
    public string Url { get; set; }
    public FastFed.Models.ProviderMetadata ProviderMetadata { get; set; }
    public IdentityProviderFederation IdentityProviderFederation { get; set; }
}
