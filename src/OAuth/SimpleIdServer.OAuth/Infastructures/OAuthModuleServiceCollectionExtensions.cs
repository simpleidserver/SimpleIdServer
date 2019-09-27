// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OAuthModuleServiceCollectionExtensions
    {
        /// <summary>
        /// Resolve module and register dependencies.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSIDModule(this IServiceCollection services)
        {
            var mvcBuilder = services.AddMvc();
            var modules = ResolveModules();
            foreach (var module in modules)
            {
                module.Register(services);
                mvcBuilder.AddApplicationPart(module.GetType().Assembly);
            }

            return services;
        }

        private static IEnumerable<IModule> ResolveModules()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IModule).IsAssignableFrom(p) && p != typeof(IModule));
            var modules = new List<IModule>();
            foreach (var type in types)
            {
                modules.Add((IModule)Activator.CreateInstance(type));
            }

            return modules;
        }
    }
}