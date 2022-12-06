// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Jwt;
using System.Text.Json;

namespace SimpleIdServer.Store.Configurations
{
    public class JsonWebKeyConfiguration : IEntityTypeConfiguration<JsonWebKey>
    {
        public void Configure(EntityTypeBuilder<JsonWebKey> builder)
        {
            builder.HasKey(j => j.Kid);
            builder.Ignore(j => j.KeyOps);
            builder.HasMany(j => j.KeyOperationLst).WithOne();
            builder.Property(j => j.Content).HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, new JsonSerializerOptions()));
        }
    }
}
