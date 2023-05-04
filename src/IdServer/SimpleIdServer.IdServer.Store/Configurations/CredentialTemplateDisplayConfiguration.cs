// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class CredentialTemplateDisplayConfiguration : IEntityTypeConfiguration<CredentialTemplateDisplay>
    {
        public void Configure(EntityTypeBuilder<CredentialTemplateDisplay> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Ignore(c => c.Logo);
        }
    }
}
