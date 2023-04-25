// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations
{
    public class UserWalletCredentialConfiguration : IEntityTypeConfiguration<UserWalletCredential>
    {
        public void Configure(EntityTypeBuilder<UserWalletCredential> builder)
        {
            builder.HasKey(w => w.Id);
            builder.Property(a => a.ContextUrls).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None).ToList());
            builder.Property(a => a.CredentialTypes).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.None).ToList());
            builder.HasMany(a => a.Claims).WithOne(u => u.WalletCredential).HasForeignKey(u => u.WalletCredentialId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
