// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.UI.Authenticate.Sms.Services;

namespace SimpleIdServer.UI.Authenticate.Sms
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSms(this IServiceCollection services)
        {
            services.AddTransient<ISmsAuthService, SmsAuthService>();
            return services;
        }
    }
}
