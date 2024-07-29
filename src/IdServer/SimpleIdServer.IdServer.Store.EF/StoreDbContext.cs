// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.Configurations;
using SimpleIdServer.IdServer.Store.EF.Configurations;
using SimpleIdServer.OpenidFederation.Domains;

namespace SimpleIdServer.IdServer.Store.EF
{
    public class StoreDbContext : DbContext, IDataProtectionKeyContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RealmUser> RealmUser { get; set; }
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
        public DbSet<UserCredential> UserCredential { get; set; }
        public DbSet<ExtractedRepresentation> ExtractedRepresentations { get; set; }
        public DbSet<ExtractedRepresentationStaging> ExtractedRepresentationsStaging { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<DeviceAuthCode> DeviceAuthCodes { get; set; }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public DbSet<UserSession> UserSession { get; set; }
        public DbSet<ConfigurationDefinition> Definitions { get; set; }
        public DbSet<ConfigurationKeyPairValueRecord> ConfigurationKeyPairValueRecords { get; set; }
        public DbSet<RegistrationWorkflow> RegistrationWorkflows { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<GroupRealm> GroupRealm { get; set; }
        public DbSet<GroupUser> GroupUser { get; set; }
        public DbSet<GotifySession> GotifySessions { get; set; }
        public DbSet<PresentationDefinition> PresentationDefinitions { get; set; }
        public DbSet<MessageBusErrorMessage> MessageBusErrorMessages { get; set; }
        public DbSet<FederationEntity> FederationEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new LanguageConfiguration());
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
            builder.ApplyConfiguration(new AuthenticationSchemeProviderMapperConfiguration());
            builder.ApplyConfiguration(new AuthenticationSchemeProviderDefinitionConfiguration());
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
            builder.ApplyConfiguration(new IdentityProvisioningDefinitionConfiguration());
            builder.ApplyConfiguration(new IdentityProvisioningMappingRuleConfiguration());
            builder.ApplyConfiguration(new IdentityProvisioningHistoryConfiguration());
            builder.ApplyConfiguration(new ExtractedRepresentationConfiguration());
            builder.ApplyConfiguration(new RealmUserConfiguration());
            builder.ApplyConfiguration(new GroupConfiguration());
            builder.ApplyConfiguration(new DeviceAuthCodeConfiguration());
            builder.ApplyConfiguration(new ConfigurationDefinitionConfiguration());
            builder.ApplyConfiguration(new ConfigurationDefinitionRecordConfiguration());
            builder.ApplyConfiguration(new ConfigurationDefinitionRecordValueConfiguration());
            builder.ApplyConfiguration(new ConfigurationKeyPairValueRecordConfiguration());
            builder.ApplyConfiguration(new RegistrationWorkflowConfiguration());
            builder.ApplyConfiguration(new AuthorizedResourceConfiguration());
            builder.ApplyConfiguration(new ExtractedRepresentationStagingConfiguration());
            builder.ApplyConfiguration(new GroupRealmConfiguration());
            builder.ApplyConfiguration(new GroupUserConfiguration());
            builder.ApplyConfiguration(new GotifySessionConfiguration());
            builder.ApplyConfiguration(new PresentationDefinitionConfiguration());
            builder.ApplyConfiguration(new PresentationDefinitionFormatConfiguration());
            builder.ApplyConfiguration(new PresentationDefinitionInputDescriptorConfiguration());
            builder.ApplyConfiguration(new PresentationDefinitionInputDescriptorConstraintConfiguration());
            builder.ApplyConfiguration(new MessageBusErrorMessageConfiguration());
            builder.ApplyConfiguration(new FederationEntityConfiguration());
            builder.ApplyConfiguration(new RealmRoleConfiguration());
            builder.ApplyConfiguration(new RealmRolePermissionConfiguration());
            builder.ApplyConfiguration(new GroupRealmRoleConfiguration());
        }
    }
}
