// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Scim.Domains;

namespace ScimShadowProperty.Repositories.Configurations
{
    public class SCIMAttributeMappingConfiguration : IEntityTypeConfiguration<SCIMAttributeMapping>
    {
        public void Configure(EntityTypeBuilder<SCIMAttributeMapping> builder)
        {
            builder.HasKey(s => s.Id);
        }
    }
}
