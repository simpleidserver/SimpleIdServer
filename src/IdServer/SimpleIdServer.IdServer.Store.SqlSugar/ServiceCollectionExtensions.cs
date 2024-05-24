// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Store.SqlSugar;
using SimpleIdServer.IdServer.Stores;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlSugarStore(this IServiceCollection services, Action<SqlSugarOptions> action)
    {
        services.Configure(action);
        services.AddScoped<DbContext>();
        services.AddTransient<IRealmRepository, RealmRepository>();
        services.AddTransient<IApiResourceRepository, ApiResourceRepository>();
        services.AddTransient<IAuditEventRepository, AuditEventRepository>();
        services.AddTransient<IAuthenticationSchemeProviderDefinitionRepository, AuthenticationSchemeProviderDefinitionRepository>();
        services.AddTransient<IAuthenticationSchemeProviderRepository, AuthenticationSchemeProviderRepository>();
        services.AddTransient<IBCAuthorizeRepository, BCAuthorizeRepository>();
        services.AddTransient<IClaimProviderRepository, ClaimProviderRepository>();
        services.AddTransient<ICertificateAuthorityRepository, CertificateAuthorityRepository>();
        services.AddTransient<IClientRepository, ClientRepository>();
        services.AddTransient<IConfigurationDefinitionStore, ConfigurationDefinitionStore>();
        services.AddTransient<IDeviceAuthCodeRepository, DeviceAuthCodeRepository>();
        services.AddTransient<IGotiySessionStore, GotiySessionStore>();
        services.AddTransient<IGrantRepository, GrantRepository>();
        services.AddTransient<IGroupRepository, GroupRepository>();
        services.AddTransient<IIdentityProvisioningStore, IdentityProvisioningStore>();
        services.AddTransient<IKeyValueRepository, KeyValueRepository>();
        services.AddTransient<ILanguageRepository, LanguageRepository>();
        services.AddTransient<IPresentationDefinitionStore, PresentationDefinitionStore>();
        services.AddTransient<IRegistrationWorkflowRepository, RegistrationWorkflowRepository>();
        services.AddTransient<IScopeRepository, ScopeRepository>();
        services.AddTransient<IFileSerializedKeyStore, SerializedFileKeyStore>();
        services.AddTransient<ITokenRepository, TokenRepository>();
        services.AddTransient<ITranslationRepository, TranslationRepository>();
        services.AddTransient<IUmaPendingRequestRepository, UmaPendingRequestRepository>();
        services.AddTransient<IUmaResourceRepository, UmaResourceRepository>();
        services.AddTransient<ITransactionBuilder, SqlSugarTransactionBuilder>();
        return services;
    }
}