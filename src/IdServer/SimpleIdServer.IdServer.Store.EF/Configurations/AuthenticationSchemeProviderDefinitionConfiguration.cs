// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class AuthenticationSchemeProviderDefinitionConfiguration : IEntityTypeConfiguration<AuthenticationSchemeProviderDefinition>
    {
        public void Configure(EntityTypeBuilder<AuthenticationSchemeProviderDefinition> builder)
        {
            builder.HasKey(a => a.Name);
            builder.HasMany(a => a.AuthSchemeProviders).WithOne(p => p.AuthSchemeProviderDefinition).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
