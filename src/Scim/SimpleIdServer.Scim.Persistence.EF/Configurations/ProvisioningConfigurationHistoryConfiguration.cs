// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Scim.Domains;

namespace SimpleIdServer.Scim.Persistence.EF.Configurations
{
    public class ProvisioningConfigurationHistoryConfiguration : IEntityTypeConfiguration<ProvisioningConfigurationHistory>
    {
        public void Configure(EntityTypeBuilder<ProvisioningConfigurationHistory> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
        }
    }
}
