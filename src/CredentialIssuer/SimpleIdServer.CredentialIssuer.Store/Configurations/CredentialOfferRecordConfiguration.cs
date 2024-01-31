// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.CredentialIssuer.Domains;

namespace SimpleIdServer.CredentialIssuer.Store.Configurations;

public class CredentialOfferRecordConfiguration : IEntityTypeConfiguration<CredentialOfferRecord>
{
    public void Configure(EntityTypeBuilder<CredentialOfferRecord> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(a => a.GrantTypes).HasConversion(
            v => string.Join(',', v),
            v => v.Split(',', StringSplitOptions.None).ToList());
        builder.Property(a => a.CredentialConfigurationIds).HasConversion(
            v => string.Join(',', v),
            v => v.Split(',', StringSplitOptions.None).ToList());
    }
}
