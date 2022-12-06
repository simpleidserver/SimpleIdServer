// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Domains;
using SimpleIdServer.Jwt;
using SimpleIdServer.Store.Configurations;

namespace SimpleIdServer.Store
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Scope> Scopes { get; set; }
        public DbSet<JsonWebKey> JsonWebKeys { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<Token> Tokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new ClientConfiguration());
            builder.ApplyConfiguration(new ConsentConfiguration());
            builder.ApplyConfiguration(new ClientScopeConfiguration());
            builder.ApplyConfiguration(new ScopeClaimConfiguration());
            builder.ApplyConfiguration(new ScopeConfiguration());
            builder.ApplyConfiguration(new TranslationConfiguration());
            builder.ApplyConfiguration(new UserClaimConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new UserCredentialConfiguration());
            builder.ApplyConfiguration(new UserExternalAuthProviderConfiguration());
            builder.ApplyConfiguration(new UserSessionConfiguration());
            builder.ApplyConfiguration(new JsonWebKeyConfiguration());
            builder.ApplyConfiguration(new JsonWebKeyKeyOperationConfigurationConfiguration());
            builder.ApplyConfiguration(new TokenConfiguration());
        }
    }
}
