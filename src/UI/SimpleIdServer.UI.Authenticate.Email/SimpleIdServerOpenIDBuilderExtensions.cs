// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.UI.Authenticate.Email;
using SimpleIdServer.UI.Authenticate.Email.Services;
using System;

namespace SimpleIdServer.OpenID
{
    public static class SimpleIdServerOpenIDBuilderExtensions
    {
        /// <summary>
        /// Register email authentication.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static SimpleIdServerOpenIDBuilder AddEmailAuthentication(this SimpleIdServerOpenIDBuilder builder, Action<EmailHostOptions> callback = null)
        {
            RegisterDependencies(builder.ServiceCollection);
            if (callback == null)
            {
                builder.ServiceCollection.Configure<EmailHostOptions>((opts) => { });
            } 
            else
            {
                builder.ServiceCollection.Configure(callback);
            }

            return builder;
        }

        internal static void RegisterDependencies(IServiceCollection services)
        {
            services.AddTransient<IEmailAuthService, EmailAuthService>();
        }
    }
}
