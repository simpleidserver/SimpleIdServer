// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Persistence.EF.Configurations
{
    public class SCIMSchemaAttributeConfiguration : IEntityTypeConfiguration<SCIMSchemaAttribute>
    {
        public void Configure(EntityTypeBuilder<SCIMSchemaAttribute> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(p => p.CanonicalValues)
                .HasConversion(p => string.Join(",", p), p => p.Split(',', System.StringSplitOptions.None).Where(s => !string.IsNullOrWhiteSpace(s)).ToList());
            builder.Property(p => p.ReferenceTypes)
                .HasConversion(p => string.Join(",", p), p => p.Split(',', System.StringSplitOptions.None).Where(s => !string.IsNullOrWhiteSpace(s)).ToList());
            builder.Property(p => p.DefaultValueString)
                .HasConversion(p => string.Join(",", p), p => p.Split(',', System.StringSplitOptions.None).Where(s => !string.IsNullOrWhiteSpace(s)).ToList());
            builder.Property(p => p.DefaultValueInt)
                .HasConversion(p => string.Join(",", p), p => string.IsNullOrWhiteSpace(p) ? new List<int>() : p.Split(',', System.StringSplitOptions.None).Select(s => int.Parse(s)).ToList());
        }
    }
}
