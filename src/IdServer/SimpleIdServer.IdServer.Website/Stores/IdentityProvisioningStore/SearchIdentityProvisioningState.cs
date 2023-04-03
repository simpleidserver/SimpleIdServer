// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.IdentityProvisioningStore
{
    [FeatureState]
    public record SearchIdentityProvisioningState
    {
        public SearchIdentityProvisioningState() { }

        public SearchIdentityProvisioningState(bool isLoading, IEnumerable<IdentityProvisioning> identityProvisioning, int nb)
        {
            Values = identityProvisioning.Select(c => new SelectableIdentityProvisioning(c));
            Count = nb;
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableIdentityProvisioning> Values { get; set; } = new List<SelectableIdentityProvisioning>();
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = false;
    }

    public class SelectableIdentityProvisioning
    {
        public SelectableIdentityProvisioning(IdentityProvisioning identityProvisioning)
        {
            Value = identityProvisioning;
        }

        public bool IsSelected { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public IdentityProvisioning Value { get; set; }
    }
}
