// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Scim.Domains;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScimShadowProperty.Repositories.Configurations
{
    public class ProvisioningConfigurationRecordConfiguration : IEntityTypeConfiguration<ProvisioningConfigurationRecord>
    {
        public void Configure(EntityTypeBuilder<ProvisioningConfigurationRecord> builder)
        {
            var strValueComparer = new ValueComparer<ICollection<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.Property(p => p.ValuesString)
                .HasConversion(p => string.Join("$", p), p => p.Split('$', StringSplitOptions.None).ToList());
            builder.Property(p => p.ValuesString).Metadata.SetValueComparer(strValueComparer);
            builder.HasMany(p => p.Values).WithOne();
        }
    }
}
