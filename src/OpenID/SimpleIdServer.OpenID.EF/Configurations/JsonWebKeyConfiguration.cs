// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using SimpleIdServer.Jwt;
using System.Collections.Generic;

namespace SimpleIdServer.OpenID.EF.Configurations
{
    public class JsonWebKeyConfiguration : IEntityTypeConfiguration<JsonWebKey>
    {
        public void Configure(EntityTypeBuilder<JsonWebKey> builder)
        {
            builder.HasKey(j => j.Kid);
            builder.Ignore(j => j.KeyOps);
            builder.HasMany(j => j.KeyOperationLst).WithOne();
            builder.Property(j => j.Content).HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v));
        }
    }
}
