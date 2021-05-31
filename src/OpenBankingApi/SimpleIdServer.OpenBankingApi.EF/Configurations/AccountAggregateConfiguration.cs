// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.OpenBankingApi.Domains;
using SimpleIdServer.OpenBankingApi.Domains.Account;
using SimpleIdServer.OpenBankingApi.Domains.Account.Enums;

namespace SimpleIdServer.OpenBankingApi.EF.Configurations
{
    public class AccountAggregateConfiguration : IEntityTypeConfiguration<AccountAggregate>
    {
        public void Configure(EntityTypeBuilder<AccountAggregate> builder)
        {
            builder.HasKey(a => a.AggregateId);
            builder.Ignore(a => a.DomainEvents);
            builder.Property(a => a.Status).HasConversion(a => a.Id, a => Enumeration.FromId<AccountStatus>(a));
            builder.Property(a => a.AccountType).HasConversion(a => a.Id, a => Enumeration.FromId<AccountTypes>(a));
            builder.Property(a => a.AccountSubType).HasConversion(a => a.Id, a => Enumeration.FromId<AccountSubTypes>(a));
            builder.Property(a => a.SwitchStatus).HasConversion(a => a.Id, a => Enumeration.FromId<AccountSwitchStatus>(a));
            builder.HasMany(a => a.Accounts).WithOne();
            builder.HasOne(a => a.Servicer).WithMany();
        }
    }
}
