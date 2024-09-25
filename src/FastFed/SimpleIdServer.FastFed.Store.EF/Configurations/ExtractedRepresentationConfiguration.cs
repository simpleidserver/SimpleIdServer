// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.FastFed.Models;

namespace SimpleIdServer.FastFed.Store.EF.Configurations;

public class ExtractedRepresentationConfiguration : IEntityTypeConfiguration<ExtractedRepresentation>
{
    public void Configure(EntityTypeBuilder<ExtractedRepresentation> builder)
    {
        builder.HasKey(e => e.Id);
    }
}
