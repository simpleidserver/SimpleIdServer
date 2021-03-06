﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenBankingApi;
using SimpleIdServer.OpenBankingApi.EF;
using SimpleIdServer.OpenBankingApi.EF.Repositories;
using SimpleIdServer.OpenBankingApi.Persistences;
using SimpleIdServer.OpenID.EF;
using SimpleIdServer.OpenID.EF.Repositories;
using SimpleIdServer.OpenID.Persistence;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static OpenBankingApiBuilder AddOpenBankingEF(this OpenBankingApiBuilder builder, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            var services = builder.ServiceCollection;
            services.AddDbContext<OpenIdDBContext>(optionsAction, ServiceLifetime.Transient);
            services.AddDbContext<OpenBankingDbContext>(optionsAction, ServiceLifetime.Transient);
            services.AddTransient<IAuthenticationContextClassReferenceRepository, AuthenticationContextClassReferenceRepository>();
            services.AddTransient<IBCAuthorizeRepository, BCAuthorizeRepository>();
            services.AddTransient<IJsonWebKeyRepository, JsonWebKeyRepository>();
            services.AddTransient<IOAuthClientRepository, OAuthClientRepository>();
            services.AddTransient<IOAuthScopeRepository, OAuthScopeRepository>();
            services.AddTransient<IOAuthUserRepository, OAuthUserRepository>();
            services.AddTransient<ITokenRepository, TokenRepository>();
            services.AddTransient<IAccountAccessConsentRepository, AccountAccessConsentRepository>();
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<ITranslationRepository, TranslationRepository>();
            return builder;
        }
    }
}
