// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.CredentialIssuer.Domains;

namespace SimpleIdServer.CredentialIssuer.Store.Configurations;

public class DeferredCredentialClaimConfiguration : IEntityTypeConfiguration<DeferredCredentialClaim>
{
    public void Configure(EntityTypeBuilder<DeferredCredentialClaim> builder)
    {
        builder.HasKey(d => d.Id);
    }
}
