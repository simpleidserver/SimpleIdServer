﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.OAuth.Domains;
using System;
namespace SimpleIdServer.OpenID.EF.Configurations
{
    public class OAuthConsentConfiguration : IEntityTypeConfiguration<OAuthConsent>
    {
        public void Configure(EntityTypeBuilder<OAuthConsent> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasMany(c => c.Scopes).WithMany(s => s.Consents);
            builder.Property(c => c.Claims).HasConversion(
                c => string.Join(',', c),
                c => c.Split(',', StringSplitOptions.None));
        }
    }
}