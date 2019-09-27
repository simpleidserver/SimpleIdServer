// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.UI.Authenticate.LoginPassword.Services;

namespace SimpleIdServer.OpenID
{
    public static class SimpleIdServerOpenIDBuilderExtensions
    {
        /// <summary>
        /// Register login password authentication.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static SimpleIdServerOpenIDBuilder AddLoginPasswordAuthentication(this SimpleIdServerOpenIDBuilder builder)
        {
            RegisterDependencies(builder.ServiceCollection);
            return builder;
        }

        internal static void RegisterDependencies(IServiceCollection services)
        {
            services.AddTransient<IPasswordAuthService, PasswordAuthService>();
        }
    }
}
