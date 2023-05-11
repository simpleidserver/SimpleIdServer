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
        public DbSet<AuthenticationSchemeProviderDefinition> AuthenticationSchemeProviderDefinitions { get; set; }
        public DbSet<AuthenticationSchemeProvider> AuthenticationSchemeProviders { get; set; }
        public DbSet<ClaimProvider> ClaimProviders { get; set; }
        public DbSet<BCAuthorize> BCAuthorizeLst { get; set; }
        public DbSet<ApiResource> ApiResources { get; set; }
        public DbSet<UMAResource> UmaResources { get; set; }
        public DbSet<UMAPendingRequest> UmaPendingRequest { get; set; }
        public DbSet<Realm> Realms { get; set; }
        public DbSet<SerializedFileKey> SerializedFileKeys { get; set; }
        public DbSet<AuditEvent> AuditEvents { get; set; }
        public DbSet<CertificateAuthority> CertificateAuthorities { get; set; }
        public DbSet<Consent> Grants { get; set; }
        public DbSet<IdentityProvisioning> IdentityProvisioningLst { get; set; }
        public DbSet<IdentityProvisioningDefinition> IdentityProvisioningDefinitions { get; set; }
        public DbSet<UserClaim> UserClaims { get; set; }
        public DbSet<ExtractedRepresentation> ExtractedRepresentations { get; set; }
        public DbSet<ImportSummary> ImportSummaries { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<CredentialTemplate> CredentialTemplates { get; set; }
        public DbSet<UserCredentialOffer> CredentialOffers { get; set; }

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
            builder.ApplyConfiguration(new UserCredentialOfferConfiguration());
            builder.ApplyConfiguration(new UserCredentialConfiguration());
            builder.ApplyConfiguration(new UserExternalAuthProviderConfiguration());
            builder.ApplyConfiguration(new UserSessionConfiguration());
            builder.ApplyConfiguration(new ClientJsonWebKeyConfiguration());
            builder.ApplyConfiguration(new TokenConfiguration());
            builder.ApplyConfiguration(new AuthenticationContextClassReferenceConfiguration());
            builder.ApplyConfiguration(new AuthenticationSchemeProviderConfiguration());
            builder.ApplyConfiguration(new AuthenticationSchemeProviderMapperConfiguration());
            builder.ApplyConfiguration(new AuthenticationSchemeProviderPropertyConfiguration());
            builder.ApplyConfiguration(new AuthenticationSchemeProviderDefinitionConfiguration());
            builder.ApplyConfiguration(new AuthenticationSchemeProviderDefinitionPropertyConfiguration());
            builder.ApplyConfiguration(new ClaimProviderConfiguration());
            builder.ApplyConfiguration(new BCAuthorizeConfiguration());
            builder.ApplyConfiguration(new BCAuthorizeHistoryConfiguration());
            builder.ApplyConfiguration(new UserDeviceConfiguration());
            builder.ApplyConfiguration(new ApiResourceConfiguration());
            builder.ApplyConfiguration(new AuthorizedScopeConfiguration());
            builder.ApplyConfiguration(new UMAPendingRequestConfiguration());
            builder.ApplyConfiguration(new UMAPermissionTicketConfiguration());
            builder.ApplyConfiguration(new UMAPermissionTicketRecordConfiguration());
            builder.ApplyConfiguration(new UmaResourceConfiguration());
            builder.ApplyConfiguration(new UMAResourcePermissionClaimConfiguration());
            builder.ApplyConfiguration(new UMAResourcePermissionConfiguration());
            builder.ApplyConfiguration(new RealmConfiguration());
            builder.ApplyConfiguration(new SerializedFileKeyConfiguration());
            builder.ApplyConfiguration(new AuditEventConfiguration());
            builder.ApplyConfiguration(new CertificateAuthorityConfiguration());
            builder.ApplyConfiguration(new ClientCertificateConfiguration());
            builder.ApplyConfiguration(new IdentityProvisioningConfiguration());
            builder.ApplyConfiguration(new IdentityProvisioningPropertyConfiguration());
            builder.ApplyConfiguration(new IdentityProvisioningDefinitionConfiguration());
            builder.ApplyConfiguration(new IdentityProvisioningDefinitionPropertyConfiguration());
            builder.ApplyConfiguration(new IdentityProvisioningMappingRuleConfiguration());
            builder.ApplyConfiguration(new IdentityProvisioningHistoryConfiguration());
            builder.ApplyConfiguration(new ExtractedRepresentationConfiguration());
            builder.ApplyConfiguration(new RealmUserConfiguration());
            builder.ApplyConfiguration(new ImportSummaryConfiguration());
            builder.ApplyConfiguration(new GroupConfiguration());
            builder.ApplyConfiguration(new BaseCredentialTemplateConfiguration());
            builder.ApplyConfiguration(new CredentialTemplateConfiguration());
            builder.ApplyConfiguration(new CredentialTemplateDisplayConfiguration());
            builder.ApplyConfiguration(new CredentialTemplateParameterConfiguration());
        }
    }
}
