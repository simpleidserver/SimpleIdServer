// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.IdentityProvisioningStore
{
    [FeatureState]
    public record UpdateIdentityProvisioningState
    {
        public UpdateIdentityProvisioningState()
        {

        }

        public UpdateIdentityProvisioningState(bool isUpdating, string errorMesasge)
        {
            IsUpdating = isUpdating;
            ErrorMessage = errorMesasge;
        }

        public bool IsUpdating { get; set; } = false;
        public string ErrorMessage { get; set; }
    }
}
