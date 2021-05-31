// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Uma.Domains;

namespace SimpleIdServer.Uma.EF.Configurations
{
    public class UMAResourceTranslationConfiguration : IEntityTypeConfiguration<UMAResourceTranslation>
    {
        public void Configure(EntityTypeBuilder<UMAResourceTranslation> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.HasOne(r => r.Translation).WithMany();
        }
    }
}
