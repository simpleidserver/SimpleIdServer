// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Configuration.Models;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers.Models;
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
        public DbSet<IdentityProvisioningProcess> IdentityProvisioningProcesses { get; set; }
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
        public DbSet<RecurringJobStatus> RecurringJobStatusLst { get; set; }
        public DbSet<DataSeederExecutionHistory> ExecutionHistories { get; set; }
        public DbSet<MigrationExecution> MigrationExecutions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new LanguageConfiguration());
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            modelBuilder.ApplyConfiguration(new ConsentConfiguration());
            modelBuilder.ApplyConfiguration(new ScopeClaimConfiguration());
            modelBuilder.ApplyConfiguration(new ScopeConfiguration());
            modelBuilder.ApplyConfiguration(new TranslationConfiguration());
            modelBuilder.ApplyConfiguration(new UserClaimConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserCredentialConfiguration());
            modelBuilder.ApplyConfiguration(new UserExternalAuthProviderConfiguration());
            modelBuilder.ApplyConfiguration(new UserSessionConfiguration());
            modelBuilder.ApplyConfiguration(new ClientJsonWebKeyConfiguration());
            modelBuilder.ApplyConfiguration(new TokenConfiguration());
            modelBuilder.ApplyConfiguration(new AuthenticationContextClassReferenceConfiguration());
            modelBuilder.ApplyConfiguration(new AuthenticationSchemeProviderConfiguration());
            modelBuilder.ApplyConfiguration(new AuthenticationSchemeProviderMapperConfiguration());
            modelBuilder.ApplyConfiguration(new AuthenticationSchemeProviderDefinitionConfiguration());
            modelBuilder.ApplyConfiguration(new ClaimProviderConfiguration());
            modelBuilder.ApplyConfiguration(new BCAuthorizeConfiguration());
            modelBuilder.ApplyConfiguration(new BCAuthorizeHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new UserDeviceConfiguration());
            modelBuilder.ApplyConfiguration(new ApiResourceConfiguration());
            modelBuilder.ApplyConfiguration(new AuthorizedScopeConfiguration());
            modelBuilder.ApplyConfiguration(new UMAPendingRequestConfiguration());
            modelBuilder.ApplyConfiguration(new UMAPermissionTicketConfiguration());
            modelBuilder.ApplyConfiguration(new UMAPermissionTicketRecordConfiguration());
            modelBuilder.ApplyConfiguration(new UmaResourceConfiguration());
            modelBuilder.ApplyConfiguration(new UMAResourcePermissionClaimConfiguration());
            modelBuilder.ApplyConfiguration(new UMAResourcePermissionConfiguration());
            modelBuilder.ApplyConfiguration(new RealmConfiguration());
            modelBuilder.ApplyConfiguration(new SerializedFileKeyConfiguration());
            modelBuilder.ApplyConfiguration(new AuditEventConfiguration());
            modelBuilder.ApplyConfiguration(new CertificateAuthorityConfiguration());
            modelBuilder.ApplyConfiguration(new ClientCertificateConfiguration());
            modelBuilder.ApplyConfiguration(new IdentityProvisioningConfiguration());
            modelBuilder.ApplyConfiguration(new IdentityProvisioningDefinitionConfiguration());
            modelBuilder.ApplyConfiguration(new IdentityProvisioningMappingRuleConfiguration());
            modelBuilder.ApplyConfiguration(new IdentityProvisioningHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new IdentityProvisioningProcessConfiguration());
            modelBuilder.ApplyConfiguration(new ExtractedRepresentationConfiguration());
            modelBuilder.ApplyConfiguration(new RealmUserConfiguration());
            modelBuilder.ApplyConfiguration(new GroupConfiguration());
            modelBuilder.ApplyConfiguration(new DeviceAuthCodeConfiguration());
            modelBuilder.ApplyConfiguration(new ConfigurationDefinitionConfiguration());
            modelBuilder.ApplyConfiguration(new ConfigurationDefinitionRecordConfiguration());
            modelBuilder.ApplyConfiguration(new ConfigurationDefinitionRecordValueConfiguration());
            modelBuilder.ApplyConfiguration(new ConfigurationKeyPairValueRecordConfiguration());
            modelBuilder.ApplyConfiguration(new RegistrationWorkflowConfiguration());
            modelBuilder.ApplyConfiguration(new AuthorizedResourceConfiguration());
            modelBuilder.ApplyConfiguration(new ExtractedRepresentationStagingConfiguration());
            modelBuilder.ApplyConfiguration(new GroupRealmConfiguration());
            modelBuilder.ApplyConfiguration(new GroupUserConfiguration());
            modelBuilder.ApplyConfiguration(new GotifySessionConfiguration());
            modelBuilder.ApplyConfiguration(new PresentationDefinitionConfiguration());
            modelBuilder.ApplyConfiguration(new PresentationDefinitionFormatConfiguration());
            modelBuilder.ApplyConfiguration(new PresentationDefinitionInputDescriptorConfiguration());
            modelBuilder.ApplyConfiguration(new PresentationDefinitionInputDescriptorConstraintConfiguration());
            modelBuilder.ApplyConfiguration(new MessageBusErrorMessageConfiguration());
            modelBuilder.ApplyConfiguration(new FederationEntityConfiguration());
            modelBuilder.ApplyConfiguration(new RecurringJobStatusConfiguration());
            modelBuilder.ApplyConfiguration(new DataSeederExecutionHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new DistributedCacheConfiguration());
            modelBuilder.ApplyConfiguration(new MigrationExecutionConfiguration());
            modelBuilder.ApplyConfiguration(new MigrationExecutionHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new ClientSecretConfiguration());
        }
    }
}
