// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Saml.Idp.Domains;

namespace SimpleIdServer.Saml.Idp.EF.Configurations
{
    public class RelyingPartyClaimMappingConfiguration : IEntityTypeConfiguration<RelyingPartyClaimMapping>
    {
        public void Configure(EntityTypeBuilder<RelyingPartyClaimMapping> builder)
        {
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");
        }
    }
}
