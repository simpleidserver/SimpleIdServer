// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Common;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register Common dependencies.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IServiceCollection AddCommonSID(this IServiceCollection services, Action<SimpleIdServerCommonOptions> callback = null)
        {
            if (callback == null)
            {
                services.Configure<SimpleIdServerCommonOptions>((opt) => { });
            }
            else
            {
                services.Configure(callback);
            }

            return services;
        }
    }
}