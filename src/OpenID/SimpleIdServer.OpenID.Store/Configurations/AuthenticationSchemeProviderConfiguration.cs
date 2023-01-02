// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.OpenID.Domains;

namespace SimpleIdServer.OpenID.Store.Configurations
{
    public class AuthenticationSchemeProviderConfiguration : IEntityTypeConfiguration<AuthenticationSchemeProvider>
    {
        public void Configure(EntityTypeBuilder<AuthenticationSchemeProvider> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Ignore(c => c.JsonOptions);
        }
    }
}
