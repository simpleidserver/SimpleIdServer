// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OAuth.EF;
using SimpleIdServer.OAuth.EF.Repositories;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.Uma;
using SimpleIdServer.Uma.EF;
using SimpleIdServer.Uma.EF.Persistence;
using SimpleIdServer.Uma.Persistence;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static SimpleIdServerUmaBuilder AddSIDUmaEF(this SimpleIdServerUmaBuilder builder, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            var services = builder.Services;
            services.AddDbContext<UMAEFDbContext>(optionsAction, ServiceLifetime.Transient);
            services.AddDbContext<OAuthDBContext>(optionsAction, ServiceLifetime.Transient);
            services.AddTransient<IJsonWebKeyRepository, JsonWebKeyRepository>();
            services.AddTransient<IOAuthClientRepository, OAuthClientRepository>();
            services.AddTransient<IOAuthScopeRepository, OAuthScopeRepository>();
            services.AddTransient<IOAuthUserRepository, OAuthUserRepository>();
            services.AddTransient<ITokenRepository, TokenRepository>();
            services.AddTransient<IUMAPendingRequestRepository, UMAPendingRequestRepository>();
            services.AddTransient<IUMAResourceRepository, UMAResourceRepository>();
            return builder;
        }
    }
}
