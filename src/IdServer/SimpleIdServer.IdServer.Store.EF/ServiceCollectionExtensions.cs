// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Configuration;
using SimpleIdServer.IdServer.Store.EF;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.OpenidFederation.Stores;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEfStore(this IServiceCollection services, Action<DbContextOptionsBuilder>? action = null)
    {
        var lifetime = ServiceLifetime.Scoped;
        RegisterDepedencies(services);
        if (action != null) services.AddDbContext<StoreDbContext>(action, lifetime);
        else services.AddDbContext<StoreDbContext>(o => o.UseInMemoryDatabase("identityServer"), lifetime);
        return services;
    }

    private static void RegisterDepedencies(IServiceCollection services)
    {
        services.AddTransient<IClientRepository, ClientRepository>();
        services.AddTransient<IScopeRepository, ScopeRepository>();
        services.AddTransient<ITokenRepository, TokenRepository>();
        services.AddTransient<ITranslationRepository, TranslationRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IAuthenticationContextClassReferenceRepository, AuthenticationContextClassReferenceRepository>();
        services.AddTransient<IAuthenticationSchemeProviderDefinitionRepository, AuthenticationSchemeProviderDefinitionRepository>();
        services.AddTransient<IAuthenticationSchemeProviderRepository, AuthenticationSchemeProviderRepository>();
        services.AddTransient<IClaimProviderRepository, ClaimProviderRepository>();
        services.AddTransient<IBCAuthorizeRepository, BCAuthorizeRepository>();
        services.AddTransient<IApiResourceRepository, ApiResourceRepository>();
        services.AddTransient<IUmaResourceRepository, UmaResourceRepository>();
        services.AddTransient<IUmaPendingRequestRepository, UmaPendingRequestRepository>();
        services.AddTransient<IRealmRepository, RealmRepository>();
        services.AddTransient<IFileSerializedKeyStore, SerializedFileKeyStore>();
        services.AddTransient<IAuditEventRepository, AuditEventRepository>();
        services.AddTransient<ICertificateAuthorityRepository, CertificateAuthorityRepository>();
        services.AddTransient<IGrantRepository, GrantRepository>();
        services.AddTransient<IIdentityProvisioningStore, IdentityProvisioningStore>();
        services.AddTransient<IGroupRepository, GroupRepository>();
        services.AddTransient<IDeviceAuthCodeRepository, DeviceAuthCodeRepository>();
        services.AddTransient<IUserSessionResitory, UserSessionRepository>();
        services.AddTransient<IConfigurationDefinitionStore, ConfigurationDefinitionStore>();
        services.AddTransient<IRegistrationWorkflowRepository, RegistrationWorkflowRepository>();
        services.AddTransient<ILanguageRepository, LanguageRepository>();
        services.AddTransient<IProvisioningStagingStore, ProvisioningStagingStore>();
        services.AddTransient<IGotiySessionStore, GotiySessionStore>();
        services.AddTransient<IPresentationDefinitionStore, PresentationDefinitionStore>();
        services.AddTransient<IKeyValueRepository, KeyValueRepository>();
        services.AddTransient<IMessageBusErrorStore, MessageBusErrorStore>();
        services.AddTransient<ITransactionBuilder, EFTransactionBuilder>();
        services.AddTransient<IFederationEntityStore, FederationEntityStore>();
        services.AddTransient<IRecurringJobStatusRepository, RecurringJobStatusRepository>();
        services.AddTransient<IDataSeederExecutionHistoryRepository, DataSeederExecutionHistoryRepository>();
        services.AddTransient<IDbMigrateService, IdServerDbMigrateService>();
        services.AddTransient<IDbMigrateService, FormbuilderDbMigrateService>();
        services.AddTransient<IMigrationStore, MigrationStore>();
    }
}
