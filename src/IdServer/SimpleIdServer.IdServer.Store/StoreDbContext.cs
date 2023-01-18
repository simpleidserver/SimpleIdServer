// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.Configurations;

namespace SimpleIdServer.IdServer.Store
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Scope> Scopes { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<AuthenticationContextClassReference> Acrs { get; set; }
        public DbSet<AuthenticationSchemeProvider> AuthenticationSchemeProviders { get; set; }
        public DbSet<ClaimProvider> ClaimProviders { get; set; }
        public DbSet<BCAuthorize> BCAuthorizeLst { get; set; }
        public DbSet<PollingDeviceMessage> PollingDeviceMessages { get; set; }
        public DbSet<ApiResource> ApiResources { get; set; }
        public DbSet<Grant> Grants { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new ClientConfiguration());
            builder.ApplyConfiguration(new ConsentConfiguration());
            builder.ApplyConfiguration(new ScopeClaimConfiguration());
            builder.ApplyConfiguration(new ScopeConfiguration());
            builder.ApplyConfiguration(new TranslationConfiguration());
            builder.ApplyConfiguration(new UserClaimConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new UserCredentialConfiguration());
            builder.ApplyConfiguration(new UserExternalAuthProviderConfiguration());
            builder.ApplyConfiguration(new UserSessionConfiguration());
            builder.ApplyConfiguration(new ClientJsonWebKeyConfiguration());
            builder.ApplyConfiguration(new TokenConfiguration());
            builder.ApplyConfiguration(new AuthenticationContextClassReferenceConfiguration());
            builder.ApplyConfiguration(new AuthenticationSchemeProviderConfiguration());
            builder.ApplyConfiguration(new ClaimProviderConfiguration());
            builder.ApplyConfiguration(new BCAuthorizePermissionConfiguration());
            builder.ApplyConfiguration(new BCAuthorizeConfiguration());
            builder.ApplyConfiguration(new UserDeviceConfiguration());
            builder.ApplyConfiguration(new PollingDeviceMessageConfiguration());
            builder.ApplyConfiguration(new ApiResourceConfiguration());
            builder.ApplyConfiguration(new GrantConfiguration());
            builder.ApplyConfiguration(new AuthorizedScopeConfiguration());
        }
    }
}
