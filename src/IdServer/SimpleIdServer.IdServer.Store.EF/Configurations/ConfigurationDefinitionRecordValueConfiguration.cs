// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Configuration.Models;

namespace SimpleIdServer.IdServer.Store.Configurations;

public class ConfigurationDefinitionRecordValueConfiguration : IEntityTypeConfiguration<ConfigurationDefinitionRecordValue>
{
    public void Configure(EntityTypeBuilder<ConfigurationDefinitionRecordValue> builder)
    {
        builder.ToTable("ConfigurationDefinitionRecordValues");
        builder.HasKey(v => v.Id);
        builder.Ignore(v => v.Translations);
        builder.HasMany(v => v.Translations).WithMany();
        builder.Ignore(v => v.Names);
        builder.Ignore(v => v.Name);
    }
}
