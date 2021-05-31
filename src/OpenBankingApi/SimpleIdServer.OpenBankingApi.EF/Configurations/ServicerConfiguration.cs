// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.OpenBankingApi.Domains;
using SimpleIdServer.OpenBankingApi.Domains.Account;
using SimpleIdServer.OpenBankingApi.Domains.Account.Enums;

namespace SimpleIdServer.OpenBankingApi.EF.Configurations
{
    public class ServicerConfiguration : IEntityTypeConfiguration<Servicer>
    {
        public void Configure(EntityTypeBuilder<Servicer> builder)
        {
            builder.HasKey(s => s.Identification);
            builder.Property(a => a.SchemeName).HasConversion(a => a.Id, a => Enumeration.FromId<ExternalAccountIdentificationTypes>(a));
        }
    }
}
