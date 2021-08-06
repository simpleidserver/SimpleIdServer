// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.Saml.Idp.Domains;

namespace SimpleIdServer.Saml.Idp.EF.Configurations
{
    public class RelyingPartyAggregateConfiguration : IEntityTypeConfiguration<RelyingPartyAggregate>
    {
        public void Configure(EntityTypeBuilder<RelyingPartyAggregate> builder)
        {
            throw new System.NotImplementedException();
        }
    }
}
