// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class IdentityProvisioning
    {
        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsEnabled { get; set; } = true;
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public IdentityProvisioningDefinition Definition { get; set; } = null!;
        public ICollection<IdentityProvisioningProperty> Properties { get; set; } = new List<IdentityProvisioningProperty>();
    }
}
