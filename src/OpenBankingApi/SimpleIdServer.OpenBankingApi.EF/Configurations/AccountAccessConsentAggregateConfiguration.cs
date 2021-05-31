// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.OpenBankingApi.Domains;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Enums;
using System;
using System.Linq;

namespace SimpleIdServer.OpenBankingApi.EF.Configurations
{
    public class AccountAccessConsentAggregateConfiguration : IEntityTypeConfiguration<AccountAccessConsentAggregate>
    {
        public void Configure(EntityTypeBuilder<AccountAccessConsentAggregate> builder)
        {
            builder.HasKey(a => a.AggregateId);
            builder.Ignore(a => a.DomainEvents);
            builder.Property(a => a.Status).HasConversion(a => a.Id, a => Enumeration.FromId<AccountAccessConsentStatus>(a));
            builder.Property(a => a.Permissions).HasConversion(
                a => string.Join(',', a.Select(_ => _.Id)),
                a => a.Split(',', StringSplitOptions.None).Select(a => Enumeration.FromId<AccountAccessConsentPermission>(int.Parse(a))).ToList());
            builder.Property(a => a.AccountIds).HasConversion(a => string.Join(',', a), a => a.Split(',', StringSplitOptions.None));
        }
    }
}
