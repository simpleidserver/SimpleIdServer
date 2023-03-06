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
    public class SCIMSchemaAttributeConfiguration : IEntityTypeConfiguration<SCIMSchemaAttribute>
    {
        public void Configure(EntityTypeBuilder<SCIMSchemaAttribute> builder)
        {
            var strValueComparer = new ValueComparer<ICollection<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());
            var intValueComparer = new ValueComparer<ICollection<int>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());
            builder.HasKey(s => s.Id);
            builder.Property(p => p.CanonicalValues)
                .HasConversion(p => string.Join(",", p), p => p.Split(',', StringSplitOptions.None).Where(s => !string.IsNullOrWhiteSpace(s)).ToList());
            builder.Property(p => p.CanonicalValues).Metadata.SetValueComparer(strValueComparer);
            builder.Property(p => p.ReferenceTypes)
                .HasConversion(p => string.Join(",", p), p => p.Split(',', StringSplitOptions.None).Where(s => !string.IsNullOrWhiteSpace(s)).ToList());
            builder.Property(p => p.ReferenceTypes).Metadata.SetValueComparer(strValueComparer);
            builder.Property(p => p.DefaultValueString)
                .HasConversion(p => string.Join(",", p), p => p.Split(',', StringSplitOptions.None).Where(s => !string.IsNullOrWhiteSpace(s)).ToList());
            builder.Property(p => p.DefaultValueString).Metadata.SetValueComparer(strValueComparer);
            builder.Property(p => p.DefaultValueInt)
                .HasConversion(p => string.Join(",", p), p => string.IsNullOrWhiteSpace(p) ? new List<int>() : p.Split(',', System.StringSplitOptions.None).Select(s => int.Parse(s)).ToList());
            builder.Property(p => p.DefaultValueInt).Metadata.SetValueComparer(intValueComparer);
        }
    }
}
