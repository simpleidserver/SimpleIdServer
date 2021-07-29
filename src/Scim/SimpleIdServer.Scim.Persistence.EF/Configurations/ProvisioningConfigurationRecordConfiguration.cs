// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Scim.Domain;
using System.Linq;

namespace SimpleIdServer.Scim.Persistence.EF.Configurations
{
    public class ProvisioningConfigurationRecordConfiguration : IEntityTypeConfiguration<ProvisioningConfigurationRecord>
    {
        public void Configure(EntityTypeBuilder<ProvisioningConfigurationRecord> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Property(p => p.ValuesString)
                .HasConversion(p => string.Join(",", p), p => p.Split(',', System.StringSplitOptions.None).ToList());
            builder.HasMany(p => p.Values).WithOne();
        }
    }
}
