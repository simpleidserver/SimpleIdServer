// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Scim.Domains;

namespace ScimShadowProperty.Repositories.Configurations
{
    public class SCIMSchemaConfiguration : IEntityTypeConfiguration<SCIMSchema>
    {
        public void Configure(EntityTypeBuilder<SCIMSchema> builder)
        {
            builder.HasKey(s => s.Id);
            builder.HasMany(s => s.SchemaExtensions).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(s => s.Attributes).WithOne().OnDelete(DeleteBehavior.Cascade).HasForeignKey(r => r.SchemaId);
            builder.Ignore(s => s.HierarchicalAttributes);
        }
    }
}
