﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.OAuth.Domains;

namespace SimpleIdServer.OpenID.EF.Configurations
{
    public class OAuthUserCredentialConfiguration : IEntityTypeConfiguration<OAuthUserCredential>
    {
        public void Configure(EntityTypeBuilder<OAuthUserCredential> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
        }
    }
}