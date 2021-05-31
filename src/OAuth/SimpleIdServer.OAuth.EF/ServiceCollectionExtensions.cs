// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.EF;
using SimpleIdServer.OAuth.EF.Repositories;
using SimpleIdServer.OAuth.Persistence;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static SimpleIdServerOAuthBuilder AdOAuthEF(this SimpleIdServerOAuthBuilder builder, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            var services = builder.Services;
            services.AddDbContext<OAuthDBContext>(optionsAction, ServiceLifetime.Transient);
            services.AddTransient<IJsonWebKeyRepository, JsonWebKeyRepository>();
            services.AddTransient<IOAuthClientRepository, OAuthClientRepository>();
            services.AddTransient<IOAuthScopeRepository, OAuthScopeRepository>();
            services.AddTransient<IOAuthUserRepository, OAuthUserRepository>();
            services.AddTransient<ITokenRepository, TokenRepository>();
            return builder;
        }
    }
}
