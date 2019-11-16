// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleIdServer.Scim;
using SimpleIdServer.Scim.Commands.Handlers;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Persistence.InMemory;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register SCIM dependencies.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static SimpleIdServerSCIMBuilder AddSIDScim(this IServiceCollection services)
        {
            var builder = new SimpleIdServerSCIMBuilder(services);
            services.AddMvc();
            services.AddCommandHandlers()
                .AddSCIMRepository()
                .AddSCIMAuthorization();
            return builder;
        }

        /// <summary>
        /// Register SCIM dependencies.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static SimpleIdServerSCIMBuilder AddSIDScim(this IServiceCollection services, Action<SCIMHostOptions> options)
        {
            services.Configure(options);
            return services.AddSIDScim();
        }

        private static IServiceCollection AddSCIMAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("QueryScimResource", p => p.RequireClaim("scope", "query_scim_resource"));
                opts.AddPolicy("AddScimResource", p => p.RequireClaim("scope", "add_scim_resource"));
                opts.AddPolicy("DeleteScimResource", p => p.RequireClaim("scope", "delete_scim_resource"));
                opts.AddPolicy("UpdateScimResource", p => p.RequireClaim("scope", "update_scim_resource"));
                opts.AddPolicy("BulkScimResource", p => p.RequireClaim("scope", "bulk_scim_resource"));
                opts.AddPolicy("UserAuthenticated", p => p.RequireAuthenticatedUser());
            });
            return services;
        }

        private static IServiceCollection AddCommandHandlers(this IServiceCollection services)
        {
            services.AddTransient<IAddRepresentationCommandHandler, AddRepresentationCommandHandler>();
            services.AddTransient<IDeleteRepresentationCommandHandler, DeleteRepresentationCommandHandler>();
            services.AddTransient<IReplaceRepresentationCommandHandler, ReplaceRepresentationCommandHandler>();
            services.AddTransient<IPatchRepresentationCommandHandler, PatchRepresentationCommandHandler>();
            services.AddTransient<ISCIMRepresentationHelper, SCIMRepresentationHelper>();
            return services;
        }

        private static IServiceCollection AddSCIMRepository(this IServiceCollection services)
        {
            var representations = new List<SCIMRepresentation>();
            var schemas = new List<SCIMSchema>();
            schemas.AddRange(new List<SCIMSchema> { SCIMConstants.StandardSchemas.UserSchema, SCIMConstants.StandardSchemas.GroupSchema, SCIMConstants.StandardSchemas.CommonSchema });
            services.TryAddSingleton<ISCIMRepresentationCommandRepository>(new DefaultSCIMRepresentationCommandRepository(representations));
            services.TryAddSingleton<ISCIMRepresentationQueryRepository>(new DefaultSCIMRepresentationQueryRepository(representations));
            services.TryAddSingleton<ISCIMSchemaCommandRepository>(new DefaultSchemaCommandRepository(schemas));
            services.TryAddSingleton<ISCIMSchemaQueryRepository>(new DefaultSchemaQueryRepository(schemas));
            return services;
        }
    }
}
