// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Vc.Models;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class BaseCredentialTemplateConfiguration : IEntityTypeConfiguration<BaseCredentialTemplate>
    {
        public void Configure(EntityTypeBuilder<BaseCredentialTemplate> builder)
        {
            builder.HasKey(c => c.TechnicalId);
            builder.HasMany(c => c.DisplayLst).WithOne(c => c.CredentialTemplate).HasForeignKey(c => c.CredentialTemplateId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(c => c.Parameters).WithOne(c => c.CredentialTemplate).HasForeignKey(c => c.CredentialTemplateId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
