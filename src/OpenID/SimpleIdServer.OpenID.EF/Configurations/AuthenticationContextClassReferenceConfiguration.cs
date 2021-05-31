// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.OpenID.Domains;

namespace SimpleIdServer.OpenID.EF.Configurations
{
    public class AuthenticationContextClassReferenceConfiguration : IEntityTypeConfiguration<AuthenticationContextClassReference>
    {
        public void Configure(EntityTypeBuilder<AuthenticationContextClassReference> builder)
        {
            builder.HasKey(a => a.Name);
            builder.Property(a => a.AuthenticationMethodReferences).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', System.StringSplitOptions.None));
        }
    }
}
