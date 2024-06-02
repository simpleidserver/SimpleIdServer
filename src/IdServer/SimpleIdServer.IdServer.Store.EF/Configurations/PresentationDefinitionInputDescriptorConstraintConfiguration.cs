// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations;

public class PresentationDefinitionInputDescriptorConstraintConfiguration : IEntityTypeConfiguration<PresentationDefinitionInputDescriptorConstraint>
{
    public void Configure(EntityTypeBuilder<PresentationDefinitionInputDescriptorConstraint> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(a => a.Path).HasConversion(
            v => string.Join(',', v),
            v => v.Split(',', StringSplitOptions.None).ToList());
    }
}
