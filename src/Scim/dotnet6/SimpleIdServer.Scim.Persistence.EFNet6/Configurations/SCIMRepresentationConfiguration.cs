// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Scim.Domains;

namespace SimpleIdServer.Scim.Persistence.EF.Configurations
{
    public class SCIMRepresentationConfiguration : IEntityTypeConfiguration<SCIMRepresentation>
    {
        public void Configure(EntityTypeBuilder<SCIMRepresentation> builder)
        {
            builder.HasKey(r => r.Id);
            builder.HasMany(r => r.FlatAttributes).WithOne(r => r.Representation).HasForeignKey(r => r.RepresentationId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(r => r.Schemas).WithMany(s => s.Representations);
            builder.Ignore(r => r.LeafAttributes);
            builder.Ignore(r => r.HierarchicalAttributes);
        }
    }
}
