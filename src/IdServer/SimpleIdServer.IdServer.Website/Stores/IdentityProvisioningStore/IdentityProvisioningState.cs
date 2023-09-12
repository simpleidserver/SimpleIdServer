// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Api.Provisioning;

namespace SimpleIdServer.IdServer.Website.Stores.IdentityProvisioningStore
{
    [FeatureState]
    public record IdentityProvisioningState
    {
        public IdentityProvisioningState() { }

        public IdentityProvisioningState(bool isLoading, IdentityProvisioningResult identityProvisioning)
        {
            IsLoading = isLoading;
            IdentityProvisioning = identityProvisioning;
        }

        public IdentityProvisioningResult? IdentityProvisioning { get; set; } = new IdentityProvisioningResult();
        public bool IsLoading { get; set; } = true;
    }
}
