// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class IdentityProvisioningDefinitionConfiguration : IEntityTypeConfiguration<IdentityProvisioningDefinition>
    {
        public void Configure(EntityTypeBuilder<IdentityProvisioningDefinition> builder)
        {
            builder.HasKey(i => i.Name);
            builder.HasMany(i => i.MappingRules).WithOne(p => p.IdentityProvisioningDefinition).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(i => i.Instances).WithOne(i => i.Definition).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
