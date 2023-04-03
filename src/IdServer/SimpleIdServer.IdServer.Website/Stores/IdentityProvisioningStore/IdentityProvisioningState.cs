// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using IdentityProvisioning = SimpleIdServer.IdServer.Domains.IdentityProvisioning;

namespace SimpleIdServer.IdServer.Website.Stores.IdentityProvisioningStore
{
    [FeatureState]
    public record IdentityProvisioningState
    {
        public IdentityProvisioningState() { }

        public IdentityProvisioningState(bool isLoading, IdentityProvisioning identityProvisioning)
        {
            IsLoading = isLoading;
            IdentityProvisioning = identityProvisioning;
        }

        public IdentityProvisioning? IdentityProvisioning { get; set; } = new IdentityProvisioning();
        public bool IsLoading { get; set; } = true;
    }
}
