// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class BCAuthorizeConfiguration : IEntityTypeConfiguration<BCAuthorize>
    {
        public void Configure(EntityTypeBuilder<BCAuthorize> builder)
        {
            builder.HasKey(bc => bc.Id);
            builder.Property(a => a.Scopes).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None).ToList());
        }
    }
}
