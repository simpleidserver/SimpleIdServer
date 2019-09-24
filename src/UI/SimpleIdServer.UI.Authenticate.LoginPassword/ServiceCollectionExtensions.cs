// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.UI.Authenticate.LoginPassword.Services;

namespace SimpleIdServer.UI.Authenticate.LoginPassword
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLoginPassword(this IServiceCollection services)
        {
            services.AddTransient<IPasswordAuthService, PasswordAuthService>();
            return services;
        }
    }
}
