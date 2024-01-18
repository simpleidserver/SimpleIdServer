// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Api.Provisioning;

namespace SimpleIdServer.IdServer.Website.Stores.IdentityProvisioningStore;

[FeatureState]
public record IdentityProvisioningMappingRuleState
{
    public IdentityProvisioningMappingRuleState() { }

    public IdentityProvisioningMappingRuleState(bool isLoading, IdentityProvisioningMappingRuleResult mapping)
    {
        IsLoading = isLoading;
        Mapping = mapping;
    }

    public IdentityProvisioningMappingRuleResult? Mapping { get; set; } = new IdentityProvisioningMappingRuleResult();
    public bool IsLoading { get; set; } = true;
}
