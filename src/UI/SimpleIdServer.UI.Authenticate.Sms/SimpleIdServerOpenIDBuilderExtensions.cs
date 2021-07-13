// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.UI.Authenticate.Sms;
using SimpleIdServer.UI.Authenticate.Sms.Services;
using System;

namespace SimpleIdServer.OpenID
{
    public static class SimpleIdServerOpenIDBuilderExtensions
    {
        /// <summary>
        /// Register sms authentication.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static SimpleIdServerOpenIDBuilder AddSMSAuthentication(this SimpleIdServerOpenIDBuilder builder, Action<SmsHostOptions> callback = null)
        {
            RegisterDependencies(builder.ServiceCollection);
            if (callback == null)
            {
                builder.ServiceCollection.Configure<SmsHostOptions>((opts) => { });
            }
            else
            {
                builder.ServiceCollection.Configure(callback);
            }

            return builder;
        }

        internal static void RegisterDependencies(IServiceCollection services)
        {
            services.AddTransient<ISmsAuthService, SmsAuthService>();
        }
    }
}
