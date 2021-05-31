// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.OAuth.Domains;

namespace SimpleIdServer.OAuth.EF.Configurations
{
    public class OAuthClientTranslationConfiguration : IEntityTypeConfiguration<OAuthClientTranslation>
    {
        public void Configure(EntityTypeBuilder<OAuthClientTranslation> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
            builder.HasOne(t => t.Translation).WithMany();
        }
    }
}
