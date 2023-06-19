// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Vc.Models;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class CredentialTemplateParameterConfiguration : IEntityTypeConfiguration<CredentialTemplateParameter>
    {
        public void Configure(EntityTypeBuilder<CredentialTemplateParameter> builder)
        {
            builder.HasKey(c => c.Id);
        }
    }
}
