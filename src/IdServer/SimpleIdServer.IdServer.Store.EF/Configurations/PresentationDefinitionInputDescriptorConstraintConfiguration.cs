// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.EF.Extensions;

namespace SimpleIdServer.IdServer.Store.Configurations;

public class PresentationDefinitionInputDescriptorConstraintConfiguration : IEntityTypeConfiguration<PresentationDefinitionInputDescriptorConstraint>
{
    public void Configure(EntityTypeBuilder<PresentationDefinitionInputDescriptorConstraint> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(a => a.Path).ConfigureSerializer();
    }
}
