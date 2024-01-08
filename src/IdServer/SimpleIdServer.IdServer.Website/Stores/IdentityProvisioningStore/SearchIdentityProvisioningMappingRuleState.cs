// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Api.Provisioning;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.IdentityProvisioningStore
{
    [FeatureState]
    public record SearchIdentityProvisioningMappingRuleState
    {
        public SearchIdentityProvisioningMappingRuleState() { }

        public SearchIdentityProvisioningMappingRuleState(bool isLoading, IEnumerable<IdentityProvisioningMappingRuleResult> mappingRules, int nb)
        {
            MappingRules = mappingRules.Select(c => new SelectableIdentityProvisioningMappingRule(c)).ToList();
            Count = nb;
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableIdentityProvisioningMappingRule>? MappingRules { get; set; } = new List<SelectableIdentityProvisioningMappingRule>();
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = true;
    }

    public class SelectableIdentityProvisioningMappingRule
    {
        public SelectableIdentityProvisioningMappingRule(IdentityProvisioningMappingRuleResult mappingRule)
        {
            Value = mappingRule;
        }

        public bool IsSelected { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public IdentityProvisioningMappingRuleResult Value { get; set; }
    }
}
