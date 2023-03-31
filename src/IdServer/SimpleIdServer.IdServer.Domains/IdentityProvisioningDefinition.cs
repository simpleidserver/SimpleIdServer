// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class IdentityProvisioningDefinition
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public ICollection<IdentityProvisioningDefinitionProperty> Properties { get; set; } = new List<IdentityProvisioningDefinitionProperty>();
        public ICollection<IdentityProvisioningMappingRule> MappingRules { get; set; } = new List<IdentityProvisioningMappingRule>();
        public ICollection<IdentityProvisioning> Instances { get; set; } = new List<IdentityProvisioning>();
    }
}
