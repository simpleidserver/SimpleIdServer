// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Store;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStore(this IServiceCollection services, Action<DbContextOptionsBuilder>? action = null)
        {
            var lifetime = ServiceLifetime.Scoped;
            RegisterDepedencies(services);
            if (action != null) services.AddDbContext<StoreDbContext>(action, lifetime);
            else services.AddDbContext<StoreDbContext>(o => o.UseInMemoryDatabase("identityServer"), lifetime);
            return services;
        }

        public static IServiceCollection AddStoreWithFactory(this IServiceCollection services, Action<DbContextOptionsBuilder>? action = null)
        {
            RegisterDepedencies(services);
            if (action != null) services.AddDbContextFactory<StoreDbContext>(action);
            else services.AddDbContextFactory<StoreDbContext>(o => o.UseInMemoryDatabase("identityServer"));
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
            services.AddTransient<IIdentityProvisioningDefinitionStore, IdentityProvisioningDefinitionStore>();
            services.AddTransient<IExtractedRepresentationRepository, ExtractedRepresentationRepository>();
            services.AddTransient<IImportSummaryStore, ImportSummaryStore>();
            services.AddTransient<IGroupRepository, GroupRepository>();
            services.AddTransient<ICredentialTemplateRepository, CredentialTemplateRepository>();
            services.AddTransient<ICredentialOfferRepository, CredentialOfferRepository>();
            services.AddTransient<IIdentityDocumentConfigurationStore, IdentityDocumentConfigurationStore>();
            services.AddTransient<IDeviceAuthCodeRepository, DeviceAuthCodeRepository>();
            services.AddTransient<IUserCredentialRepository, UserCredentialRepository>();
        }
    }
}
