// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SimpleIdServer.CredentialIssuer.Domains;
using SimpleIdServer.CredentialIssuer.Store.Configurations;

namespace SimpleIdServer.CredentialIssuer.Store;

public class CredentialIssuerDbContext : DbContext
{
    public CredentialIssuerDbContext(DbContextOptions<CredentialIssuerDbContext> options) : base(options) { }

    public DbSet<Domains.CredentialConfiguration> CredentialConfigurations { get; set; }

    public DbSet<CredentialOfferRecord> CredentialOfferRecords { get; set; }

    public DbSet<Credential> Credentials { get; set; }

    public DbSet<UserCredentialClaim> UserCredentialClaims { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new Configurations.CredentialConfiguration());
        modelBuilder.ApplyConfiguration(new CredentialConfigurationClaimConfiguration());
        modelBuilder.ApplyConfiguration(new CredentialConfigurationConf());
        modelBuilder.ApplyConfiguration(new CredentialConfigurationTranslationConfiguration());
        modelBuilder.ApplyConfiguration(new CredentialOfferRecordConfiguration());
        modelBuilder.ApplyConfiguration(new UserCredentialClaimConfiguration());
    }
}
