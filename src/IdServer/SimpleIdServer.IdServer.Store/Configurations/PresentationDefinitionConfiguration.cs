// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations;

public class PresentationDefinitionConfiguration : IEntityTypeConfiguration<PresentationDefinition>
{
    public void Configure(EntityTypeBuilder<PresentationDefinition> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasMany(p => p.InputDescriptors).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}
