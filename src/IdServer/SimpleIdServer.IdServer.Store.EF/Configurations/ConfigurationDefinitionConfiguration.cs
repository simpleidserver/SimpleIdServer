// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations;

public class ConfigurationDefinitionConfiguration : IEntityTypeConfiguration<ConfigurationDefinition>
{
    public void Configure(EntityTypeBuilder<ConfigurationDefinition> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasMany(c => c.Records).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}
