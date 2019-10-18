// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleIdServer.Uma;
using SimpleIdServer.Uma.Domains;
using SimpleIdServer.Uma.Persistence;
using SimpleIdServer.Uma.Persistence.InMemory;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register UMA2.0 dependencies.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static SimpleIdServerUmaBuilder AddSIDUma(this IServiceCollection services)
        {
            var builder = new SimpleIdServerUmaBuilder(services);
            services.AddSIDOAuth();
            services.AddUMAStore();
            return builder;
        }

        /// <summary>
        /// Register UMA2.0 dependencies.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static SimpleIdServerUmaBuilder AddSIDUma(this IServiceCollection services, Action<UMAHostOptions> options)
        {
            services.Configure(options);
            return services.AddSIDUma();
        }

        private static IServiceCollection AddUMAStore(this IServiceCollection services)
        {
            var umaResources = new List<UMAResource>();
            services.TryAddSingleton<IUMAResourceCommandRepository>(new DefaultUMAResourceCommandRepository(umaResources));
            services.TryAddSingleton<IUMAResourceQueryRepository>(new DefaultUMAResourceQueryRepository(umaResources));
            return services;
        }
    }
}