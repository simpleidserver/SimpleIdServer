// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class IdentityProvisioningDefinitionPropertyConfiguration : IEntityTypeConfiguration<IdentityProvisioningDefinitionProperty>
    {
        public void Configure(EntityTypeBuilder<IdentityProvisioningDefinitionProperty> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
        }
    }
}
