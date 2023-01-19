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
            services.AddTransient<IClientRepository, ClientRepository>();
            services.AddTransient<IScopeRepository, ScopeRepository>();
            services.AddTransient<ITokenRepository, TokenRepository>();
            services.AddTransient<ITranslationRepository, TranslationRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IAuthenticationContextClassReferenceRepository, AuthenticationContextClassReferenceRepository>();
            services.AddTransient<IAuthenticationSchemeProviderRepository, AuthenticationSchemeProviderRepository>();
            services.AddTransient<IClaimProviderRepository, ClaimProviderRepository>();
            services.AddTransient<IBCAuthorizeRepository, BCAuthorizeRepository>();
            services.AddTransient<IApiResourceRepository, ApiResourceRepository>();
            services.AddTransient<IGrantRepository, GrantRepository>();
            if (action != null) services.AddDbContext<StoreDbContext>(action);
            else services.AddDbContext<StoreDbContext>(o => o.UseInMemoryDatabase("identityServer"));
            return services;
        }
    }
}
