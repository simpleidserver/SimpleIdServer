// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations;

public class ConfigurationDefinitionRecordConfiguration : IEntityTypeConfiguration<ConfigurationDefinitionRecord>
{
    public void Configure(EntityTypeBuilder<ConfigurationDefinitionRecord> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Translations).WithMany();
        builder.Ignore(x => x.DisplayName);
        builder.Ignore(x => x.DisplayNames);
        builder.Ignore(x => x.Description);
        builder.Ignore(x => x.Descriptions);
    }
}
