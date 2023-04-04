// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class IdentityProvisioningDefinitionProperty : IPropertyDefinition
    {
        public string PropertyName { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string? Description { get; set; } = null;
        public IdentityProvisioningDefinition IdentityProvisioningDefinition { get; set; } = null!;
    }
}
