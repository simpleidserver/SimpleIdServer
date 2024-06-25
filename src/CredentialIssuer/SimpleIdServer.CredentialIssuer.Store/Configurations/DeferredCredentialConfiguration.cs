// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.CredentialIssuer.Domains;
using System.Text.Json;

namespace SimpleIdServer.CredentialIssuer.Store.Configurations;

public class DeferredCredentialConfiguration : IEntityTypeConfiguration<DeferredCredential>
{
    public void Configure(EntityTypeBuilder<DeferredCredential> builder)
    {
        builder.HasKey(d => d.TransactionId);
        builder.HasMany(d => d.Claims).WithOne().HasForeignKey(d => d.DeferredCredentialId);
    }
}
